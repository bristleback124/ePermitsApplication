using ePermits.Data;
using ePermits.Models;
using ePermits.Models.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Models.EmailModels;
using ePermitsApp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ePermits.Services
{
    public class ChatService : IChatService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<ChatService> _logger;

        public ChatService(
            IMessageRepository messageRepository,
            IApplicationRepository applicationRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            ILogger<ChatService> logger)
        {
            _messageRepository = messageRepository;
            _applicationRepository = applicationRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesAsync(int applicationId, int userId, string userRole)
        {
            await ValidateUnreadAccessAsync(applicationId, userId, userRole);

            var messages = await _messageRepository
                .GetByApplicationIdAsync(applicationId);

            return messages.Select(m => new MessageDto
            {
                Id = m.Id,
                ApplicationId = m.ApplicationId,
                SenderId = m.SenderId,
                SenderName = $"{m.Sender?.UserProfile?.FirstName} {m.Sender?.UserProfile?.LastName}".Trim(),
                SenderType = m.SenderType,
                SenderRole = m.Sender?.UserRole?.UserRoleDesc ?? string.Empty,
                Content = m.Content,
                Timestamp = m.Timestamp,
                IsRead = IsMessageReadForActor(m, userId, userRole)
            });
        }

        public async Task<MessageDto> SendMessageAsync(SendMessageDto dto, int userId, string userRole)
        {
            var application = await ValidateUnreadAccessAsync(dto.ApplicationId, userId, userRole);

            // Admin and User roles send as "Government", Applicant sends as "Applicant"
            var senderType = (userRole == "admin" || userRole == "user") ? "Government" : "Applicant";

            var message = new Message
            {
                ApplicationId = dto.ApplicationId,
                SenderId = userId,
                SenderType = senderType,
                Content = dto.Content,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            var savedMessage = await _messageRepository.AddAsync(message);
            var recipients = await ResolveUnreadRecipientUserIdsAsync(application, userRole);

            await _messageRepository.AddRecipientStatesAsync(
                recipients.Select(recipient => new MessageRecipientState
                {
                    MessageId = savedMessage.Id,
                    RecipientUserId = recipient.UserId,
                    RecipientRole = recipient.RecipientRole,
                    SenderType = senderType,
                    IsRead = false
                }));

            await SendNewMessageEmailsAsync(application, savedMessage, recipients);

            return new MessageDto
            {
                Id = savedMessage.Id,
                ApplicationId = savedMessage.ApplicationId,
                SenderId = savedMessage.SenderId,
                SenderName = $"{savedMessage.Sender?.UserProfile?.FirstName} {savedMessage.Sender?.UserProfile?.LastName}".Trim(),
                SenderType = savedMessage.SenderType,
                SenderRole = savedMessage.Sender?.UserRole?.UserRoleDesc ?? userRole,
                Content = savedMessage.Content,
                Timestamp = savedMessage.Timestamp,
                IsRead = true
            };
        }

        public async Task MarkAsReadAsync(int applicationId, int userId, string userRole)
        {
            await ValidateUnreadAccessAsync(applicationId, userId, userRole);
            await _messageRepository.MarkAsReadAsync(
                applicationId,
                userId,
                GetUnreadSenderTypeForActor(userRole));
        }

        public async Task<int> GetUnreadCountAsync(int applicationId, int userId, string userRole)
        {
            await ValidateUnreadAccessAsync(applicationId, userId, userRole);
            return await _messageRepository.GetUnreadCountAsync(
                applicationId,
                userId,
                GetUnreadSenderTypeForActor(userRole));
        }

        private async Task<Application> ValidateUnreadAccessAsync(int applicationId, int userId, string userRole)
        {
            var application = await _applicationRepository.GetUnreadScopeApplicationAsync(applicationId);
            if (application == null)
                throw new ArgumentException("Application not found.");

            if (string.Equals(userRole, "applicant", StringComparison.OrdinalIgnoreCase))
            {
                if (application.UserId != userId)
                    throw new UnauthorizedAccessException("Access denied to this application.");

                return application;
            }

            if (string.Equals(userRole, "user", StringComparison.OrdinalIgnoreCase))
            {
                var isAssignedReviewer = application.DepartmentReviews.Any(review => review.AssignedReviewerId == userId);
                if (!isAssignedReviewer)
                    throw new UnauthorizedAccessException("Access denied to this application.");

                return application;
            }

            if (string.Equals(userRole, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return application;
            }

            throw new UnauthorizedAccessException("Access denied to this application.");
        }

        private async Task<List<UnreadRecipientTarget>> ResolveUnreadRecipientUserIdsAsync(Application application, string userRole)
        {
            if (string.Equals(userRole, "applicant", StringComparison.OrdinalIgnoreCase))
            {
                var assignedReviewerRecipients = application.DepartmentReviews
                    .Where(review => review.AssignedReviewerId.HasValue)
                    .Select(review => new UnreadRecipientTarget(
                        review.AssignedReviewerId!.Value,
                        review.AssignedReviewer?.UserRole?.UserRoleDesc ?? "user"));

                var adminRecipients = (await _userRepository.GetAllAsync())
                    .Where(user => string.Equals(user.UserRole?.UserRoleDesc, "admin", StringComparison.OrdinalIgnoreCase))
                    .Select(user => new UnreadRecipientTarget(user.Id, user.UserRole?.UserRoleDesc ?? "admin"));

                return assignedReviewerRecipients
                    .Concat(adminRecipients)
                    .GroupBy(recipient => recipient.UserId)
                    .Select(group => group.First())
                    .ToList();
            }

            return new List<UnreadRecipientTarget>
            {
                new(application.UserId, "applicant")
            };
        }

        private static string GetUnreadSenderTypeForActor(string userRole)
        {
            return string.Equals(userRole, "applicant", StringComparison.OrdinalIgnoreCase)
                ? "Government"
                : "Applicant";
        }

        private static bool IsMessageReadForActor(Message message, int userId, string userRole)
        {
            var actorSenderType = string.Equals(userRole, "applicant", StringComparison.OrdinalIgnoreCase)
                ? "Applicant"
                : "Government";

            if (string.Equals(message.SenderType, actorSenderType, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var recipientState = message.RecipientStates
                .FirstOrDefault(state => state.RecipientUserId == userId);

            return recipientState?.IsRead ?? message.IsRead;
        }

        private async Task SendNewMessageEmailsAsync(
            Application application,
            Message savedMessage,
            List<UnreadRecipientTarget> recipients)
        {
            try
            {
                var senderProfile = savedMessage.Sender?.UserProfile;
                var senderName = senderProfile != null
                    ? $"{senderProfile.FirstName} {senderProfile.LastName}".Trim()
                    : savedMessage.Sender?.Username ?? "Unknown";

                var senderRole = savedMessage.SenderType;

                var appTypeLabel = application.Type switch
                {
                    "BuildingPermit" => "Building Permit",
                    "CertificateOfOccupancy" => "Certificate of Occupancy",
                    _ => "Application"
                };

                var preview = savedMessage.Content.Length > 200
                    ? savedMessage.Content[..200] + "..."
                    : savedMessage.Content;

                foreach (var recipient in recipients)
                {
                    var user = await _userRepository.GetByIdAsync(recipient.UserId);
                    var email = user?.UserProfile?.Email;
                    if (string.IsNullOrWhiteSpace(email)) continue;

                    var recipientName = $"{user!.UserProfile!.FirstName} {user.UserProfile.LastName}".Trim();

                    await _emailService.SendTemplatedEmailAsync(
                        email,
                        $"New Message on Application {application.FormattedId}",
                        "NewChatMessage",
                        new NewChatMessageModel
                        {
                            RecipientName = recipientName,
                            SenderName = senderName,
                            SenderRole = senderRole,
                            MessagePreview = preview,
                            ApplicationFormattedId = application.FormattedId,
                            ApplicationType = appTypeLabel,
                            SentAt = savedMessage.Timestamp
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send new message email notifications for application {ApplicationId}", application.Id);
            }
        }

        private sealed record UnreadRecipientTarget(int UserId, string RecipientRole);
    }
}

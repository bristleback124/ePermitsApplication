using ePermits.Data;
using ePermits.Models;
using ePermits.Models.DTOs;

namespace ePermits.Services
{
    public class ChatService : IChatService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;

        public ChatService(
            IMessageRepository messageRepository,
            IApplicationRepository applicationRepository,
            IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _applicationRepository = applicationRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesAsync(int applicationId, int userId, string userRole)
        {
            await ValidateApplicationAccessAsync(applicationId, userId, userRole);

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
                IsRead = m.IsRead
            });
        }

        public async Task<MessageDto> SendMessageAsync(SendMessageDto dto, int userId, string userRole)
        {
            await ValidateApplicationAccessAsync(dto.ApplicationId, userId, userRole);

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
                IsRead = savedMessage.IsRead
            };
        }

        public async Task MarkAsReadAsync(int applicationId, int userId, string userRole, string senderType)
        {
            await ValidateApplicationAccessAsync(applicationId, userId, userRole);
            await _messageRepository.MarkAsReadAsync(applicationId, senderType);
        }

        public async Task<int> GetUnreadCountAsync(int applicationId, int userId, string userRole, string senderType)
        {
            await ValidateApplicationAccessAsync(applicationId, userId, userRole);
            return await _messageRepository.GetUnreadCountAsync(applicationId, senderType);
        }

        private async Task ValidateApplicationAccessAsync(int applicationId, int userId, string userRole)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);
            if (application == null)
                throw new ArgumentException("Application not found.");

            if (userRole == "applicant" && application.UserId != userId)
                throw new UnauthorizedAccessException("Access denied to this application.");
        }
    }
}

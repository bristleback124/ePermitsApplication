using ePermits.Hubs;
using ePermits.Models.DTOs;
using ePermits.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ePermits.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }

        [HttpGet("{applicationId}/messages")]
        public async Task<IActionResult> GetMessages(int applicationId)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "applicant";

            var messages = await _chatService.GetMessagesAsync(
                applicationId,
                userId,
                userRole);

            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "applicant";

            // Save message to DB
            var messageDto = await _chatService.SendMessageAsync(dto, userId, userRole);

            // Send real-time update via SignalR
            await _hubContext.Clients.Group($"application-{dto.ApplicationId}")
                .SendAsync("ReceiveMessage", messageDto);

            return Ok(messageDto);
        }

        [HttpPut("{applicationId}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int applicationId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "applicant";
            // Admin and User (govt roles) read applicant messages, Applicant reads govt messages
            var senderType = (userRole == "admin" || userRole == "user") ? "Applicant" : "Government";
            await _chatService.MarkAsReadAsync(applicationId, senderType);
            return Ok();
        }

        [HttpGet("{applicationId}/unread-count")]
        public async Task<IActionResult> GetUnreadCount(int applicationId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "applicant";
            // Admin and User (govt roles) count unread applicant messages, Applicant counts govt messages
            var senderType = (userRole == "admin" || userRole == "user") ? "Applicant" : "Government";
            var count = await _chatService.GetUnreadCountAsync(applicationId, senderType);
            return Ok(new { UnreadCount = count });
        }
    }
}

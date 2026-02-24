using ePermits.Models.DTOs;

namespace ePermits.Services
{
    public interface IChatService
    {
        Task<IEnumerable<MessageDto>> GetMessagesAsync(int applicationId, int userId, string userRole);
        Task<MessageDto> SendMessageAsync(SendMessageDto dto, int userId, string userRole);
        Task MarkAsReadAsync(int applicationId, string senderType);
        Task<int> GetUnreadCountAsync(int applicationId, string senderType);
    }
}

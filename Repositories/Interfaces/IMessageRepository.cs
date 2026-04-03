using ePermits.Models;
using ePermitsApp.Entities;

namespace ePermits.Data
{
    public interface IMessageRepository
    {
        Task<Message?> GetByIdAsync(int id);
        Task<Message?> GetDetailedByIdAsync(int id);
        Task<IEnumerable<Message>> GetByApplicationIdAsync(int applicationId);
        Task<Message> AddAsync(Message message);
        Task AddRecipientStatesAsync(IEnumerable<MessageRecipientState> recipientStates);
        Task UpdateAsync(Message message);
        Task DeleteAsync(int id);
        Task<int> GetUnreadCountAsync(int applicationId, int recipientUserId, string senderType);
        Task MarkAsReadAsync(int applicationId, int recipientUserId, string senderType);
    }
}

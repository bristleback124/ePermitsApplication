using ePermits.Models;
using ePermitsApp.Data;
using ePermitsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ePermits.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Message?> GetByIdAsync(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<Message?> GetDetailedByIdAsync(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                    .ThenInclude(s => s!.UserProfile)
                .Include(m => m.Sender)
                    .ThenInclude(s => s!.UserRole)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Message>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.Messages
                .Where(m => m.ApplicationId == applicationId)
                .Include(m => m.Sender)
                    .ThenInclude(s => s!.UserProfile)
                .Include(m => m.Sender)
                    .ThenInclude(s => s!.UserRole)
                .Include(m => m.RecipientStates)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<Message> AddAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return await GetDetailedByIdAsync(message.Id) ?? message;
        }

        public async Task AddRecipientStatesAsync(IEnumerable<MessageRecipientState> recipientStates)
        {
            var states = recipientStates.ToList();
            if (states.Count == 0)
            {
                return;
            }

            _context.MessageRecipientStates.AddRange(states);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var message = await GetByIdAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadCountAsync(int applicationId, int recipientUserId, string senderType)
        {
            return await _context.MessageRecipientStates
                .Where(state =>
                    state.RecipientUserId == recipientUserId &&
                    state.SenderType == senderType &&
                    !state.IsRead &&
                    state.Message != null &&
                    state.Message.ApplicationId == applicationId)
                .CountAsync();
        }

        public async Task MarkAsReadAsync(int applicationId, int recipientUserId, string senderType)
        {
            var recipientStates = await _context.MessageRecipientStates
                .Where(state =>
                    state.RecipientUserId == recipientUserId &&
                    state.SenderType == senderType &&
                    !state.IsRead &&
                    state.Message != null &&
                    state.Message.ApplicationId == applicationId)
                .ToListAsync();

            foreach (var recipientState in recipientStates)
            {
                recipientState.IsRead = true;
                recipientState.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}

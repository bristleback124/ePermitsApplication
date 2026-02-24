using ePermits.Models;
using ePermitsApp.Data;
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

        public async Task<IEnumerable<Message>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.Messages
                .Where(m => m.ApplicationId == applicationId)
                .Include(m => m.Sender)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<Message> AddAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
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

        public async Task<int> GetUnreadCountAsync(int applicationId, string senderType)
        {
            return await _context.Messages
                .Where(m => m.ApplicationId == applicationId && m.SenderType != senderType && !m.IsRead)
                .CountAsync();
        }

        public async Task MarkAsReadAsync(int applicationId, string senderType)
        {
            var messages = await _context.Messages
                .Where(m => m.ApplicationId == applicationId && m.SenderType != senderType && !m.IsRead)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}

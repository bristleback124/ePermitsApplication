using ePermits.Models;
using ePermitsApp.Data;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class ApplicationNoteRepository : IApplicationNoteRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationNoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApplicationNote>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.ApplicationNotes
                .Include(n => n.CreatedBy)
                    .ThenInclude(u => u!.UserProfile)
                .Where(n => n.ApplicationId == applicationId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationNote>> GetVisibleByApplicationIdAsync(int applicationId)
        {
            return await _context.ApplicationNotes
                .Include(n => n.CreatedBy)
                    .ThenInclude(u => u!.UserProfile)
                .Where(n => n.ApplicationId == applicationId && n.IsVisibleToApplicant)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<ApplicationNote?> GetByIdAsync(int id)
        {
            return await _context.ApplicationNotes
                .Include(n => n.CreatedBy)
                    .ThenInclude(u => u!.UserProfile)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<ApplicationNote> AddAsync(ApplicationNote note)
        {
            _context.ApplicationNotes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task UpdateAsync(ApplicationNote note)
        {
            _context.ApplicationNotes.Update(note);
            await _context.SaveChangesAsync();
        }
    }
}

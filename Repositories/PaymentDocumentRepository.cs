using ePermits.Models;
using ePermitsApp.Data;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class PaymentDocumentRepository : IPaymentDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentDocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentDocument>> GetByApplicationIdAsync(int applicationId)
        {
            return await _context.PaymentDocuments
                .Include(d => d.UploadedBy)
                    .ThenInclude(u => u!.UserProfile)
                .Where(d => d.ApplicationId == applicationId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<PaymentDocument?> GetByIdAsync(int id)
        {
            return await _context.PaymentDocuments
                .Include(d => d.UploadedBy)
                    .ThenInclude(u => u!.UserProfile)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<PaymentDocument> AddAsync(PaymentDocument document)
        {
            _context.PaymentDocuments.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task DeleteAsync(PaymentDocument document)
        {
            _context.PaymentDocuments.Remove(document);
            await _context.SaveChangesAsync();
        }
    }
}

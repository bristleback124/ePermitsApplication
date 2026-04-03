using ePermits.Models;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IPaymentDocumentRepository
    {
        Task<IEnumerable<PaymentDocument>> GetByApplicationIdAsync(int applicationId);
        Task<PaymentDocument?> GetByIdAsync(int id);
        Task<PaymentDocument> AddAsync(PaymentDocument document);
        Task DeleteAsync(PaymentDocument document);
    }
}

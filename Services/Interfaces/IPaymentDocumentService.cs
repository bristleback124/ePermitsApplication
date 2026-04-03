using ePermits.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace ePermitsApp.Services.Interfaces
{
    public interface IPaymentDocumentService
    {
        Task<IEnumerable<PaymentDocumentDto>> GetByApplicationIdAsync(int applicationId);
        Task<PaymentDocumentDto> UploadAsync(int applicationId, IFormFile file, int uploadedById);
        Task<bool> DeleteAsync(int documentId);
    }
}

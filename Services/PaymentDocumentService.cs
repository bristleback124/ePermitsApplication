using ePermits.Models;
using ePermits.Models.DTOs;
using ePermitsApp.Helpers;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace ePermitsApp.Services
{
    public class PaymentDocumentService : IPaymentDocumentService
    {
        private readonly IPaymentDocumentRepository _repository;
        private readonly FileStorageSettings _fileSettings;

        public PaymentDocumentService(
            IPaymentDocumentRepository repository,
            IOptions<FileStorageSettings> fileSettings)
        {
            _repository = repository;
            _fileSettings = fileSettings.Value;
        }

        public async Task<IEnumerable<PaymentDocumentDto>> GetByApplicationIdAsync(int applicationId)
        {
            var documents = await _repository.GetByApplicationIdAsync(applicationId);
            return documents.Select(MapToDto);
        }

        public async Task<PaymentDocumentDto> UploadAsync(int applicationId, IFormFile file, int uploadedById)
        {
            var folderPath = Path.Combine(_fileSettings.BasePath, "permits", applicationId.ToString(), "payment-docs");
            Directory.CreateDirectory(folderPath);

            var savedFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var fullPath = Path.Combine(folderPath, savedFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new PaymentDocument
            {
                ApplicationId = applicationId,
                UploadedById = uploadedById,
                FileName = file.FileName,
                FileSize = file.Length,
                FilePath = fullPath,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(document);

            var saved = await _repository.GetByIdAsync(document.Id);
            return MapToDto(saved!);
        }

        public async Task<bool> DeleteAsync(int documentId)
        {
            var document = await _repository.GetByIdAsync(documentId);
            if (document == null) return false;

            try
            {
                if (File.Exists(document.FilePath))
                    File.Delete(document.FilePath);
            }
            catch
            {
                // File deletion failure should not prevent record removal
            }

            await _repository.DeleteAsync(document);
            return true;
        }

        private static PaymentDocumentDto MapToDto(PaymentDocument document)
        {
            var profile = document.UploadedBy?.UserProfile;
            var uploadedByName = profile != null
                ? $"{profile.FirstName} {profile.LastName}".Trim()
                : document.UploadedBy?.Username ?? "Unknown";

            return new PaymentDocumentDto
            {
                Id = document.Id,
                ApplicationId = document.ApplicationId,
                UploadedById = document.UploadedById,
                UploadedByName = uploadedByName,
                FileName = document.FileName,
                FileSize = document.FileSize,
                FilePath = document.FilePath,
                CreatedAt = document.CreatedAt
            };
        }
    }
}

using Microsoft.AspNetCore.Http;

namespace ePermitsApp.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task<Stream> DownloadAsync(string fileName, CancellationToken cancellationToken = default);
        Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);
    }
}

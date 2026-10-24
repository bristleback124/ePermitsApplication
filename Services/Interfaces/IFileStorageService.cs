using Microsoft.AspNetCore.Http;

namespace ePermitsApp.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task<Stream> DownloadAsync(string fileName, CancellationToken cancellationToken = default);
        Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);

        /// Stores one piece of a file uploaded in chunks; pieces become the file on CommitChunksAsync.
        Task StageChunkAsync(string fileName, int index, Stream content, CancellationToken cancellationToken = default);

        /// Joins chunks 0..chunkCount-1 into `fileName` and returns its size in bytes.
        Task<long> CommitChunksAsync(string fileName, int chunkCount, CancellationToken cancellationToken = default);

        /// Size of a stored file, or null if it doesn't exist.
        Task<long?> GetSizeAsync(string fileName, CancellationToken cancellationToken = default);

        /// Sets a blob index tag where the provider supports it (used to find abandoned uploads); no-op otherwise.
        Task TrySetStatusTagAsync(string fileName, string status, CancellationToken cancellationToken = default);
    }
}

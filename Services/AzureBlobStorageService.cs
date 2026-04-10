using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ePermitsApp.Helpers;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ePermitsApp.Services
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorageService(IOptions<FileStorageSettings> settings)
        {
            var value = settings.Value;

            if (string.IsNullOrWhiteSpace(value.ConnectionString))
            {
                throw new InvalidOperationException("FileStorage:ConnectionString is not configured.");
            }

            if (string.IsNullOrWhiteSpace(value.ContainerName))
            {
                throw new InvalidOperationException("FileStorage:ContainerName is not configured.");
            }

            _containerClient = new BlobContainerClient(value.ConnectionString, value.ContainerName);
        }

        public async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (file.Length == 0) throw new InvalidOperationException("Cannot upload an empty file.");

            await _containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

            var safeOriginalName = Path.GetFileName(file.FileName);
            var storedFileName = $"{Guid.NewGuid()}_{safeOriginalName}";
            var blobClient = _containerClient.GetBlobClient(storedFileName);

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, cancellationToken: cancellationToken);

            return storedFileName;
        }

        public async Task<Stream> DownloadAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var storedFileName = NormalizeStoredFileName(fileName);
            var blobClient = _containerClient.GetBlobClient(storedFileName);
            var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
            return response.Value.Content;
        }

        public async Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var storedFileName = NormalizeStoredFileName(fileName);
            var blobClient = _containerClient.GetBlobClient(storedFileName);
            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        private static string NormalizeStoredFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name is required.", nameof(fileName));
            }

            return Path.GetFileName(fileName.Trim());
        }
    }
}

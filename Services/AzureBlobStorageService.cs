using System.Text;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using ePermitsApp.Helpers;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ePermitsApp.Services
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<AzureBlobStorageService> _logger;

        public AzureBlobStorageService(IOptions<FileStorageSettings> settings, ILogger<AzureBlobStorageService> logger)
        {
            _logger = logger;
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

        public async Task StageChunkAsync(string fileName, int index, Stream content, CancellationToken cancellationToken = default)
        {
            await _containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
            var blockClient = _containerClient.GetBlockBlobClient(NormalizeStoredFileName(fileName));
            await blockClient.StageBlockAsync(BlockId(index), content, cancellationToken: cancellationToken);
        }

        public async Task<long> CommitChunksAsync(string fileName, int chunkCount, CancellationToken cancellationToken = default)
        {
            var blockClient = _containerClient.GetBlockBlobClient(NormalizeStoredFileName(fileName));
            var blockIds = Enumerable.Range(0, chunkCount).Select(BlockId).ToList();
            try
            {
                await blockClient.CommitBlockListAsync(blockIds, cancellationToken: cancellationToken);
            }
            catch (RequestFailedException ex) when (ex.Status == 400)
            {
                throw new InvalidOperationException("Upload is incomplete: some parts never arrived.", ex);
            }
            var properties = await blockClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            return properties.Value.ContentLength;
        }

        public async Task<long?> GetSizeAsync(string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var properties = await _containerClient.GetBlobClient(NormalizeStoredFileName(fileName)).GetPropertiesAsync(cancellationToken: cancellationToken);
                return properties.Value.ContentLength;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task TrySetStatusTagAsync(string fileName, string status, CancellationToken cancellationToken = default)
        {
            try
            {
                await _containerClient.GetBlobClient(NormalizeStoredFileName(fileName)).SetTagsAsync(
                    new Dictionary<string, string> { ["status"] = status },
                    cancellationToken: cancellationToken);
            }
            catch (RequestFailedException ex)
            {
                // Tags only feed an optional cleanup rule; never fail an upload over them.
                _logger.LogWarning(ex, "Could not tag blob {FileName} as {Status}.", fileName, status);
            }
        }

        // Block IDs must all be the same length within a blob.
        private static string BlockId(int index) => Convert.ToBase64String(Encoding.ASCII.GetBytes(index.ToString("D6")));

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

using ePermitsApp.Helpers;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ePermitsApp.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly FileStorageSettings _settings;

        public LocalFileStorageService(IOptions<FileStorageSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (file.Length == 0) throw new InvalidOperationException("Cannot upload an empty file.");

            if (string.IsNullOrWhiteSpace(_settings.BasePath))
            {
                throw new InvalidOperationException("FileStorage:BasePath is not configured.");
            }

            Directory.CreateDirectory(_settings.BasePath);

            var safeOriginalName = Path.GetFileName(file.FileName);
            var storedFileName = $"{Guid.NewGuid()}_{safeOriginalName}";
            var fullPath = Path.Combine(_settings.BasePath, storedFileName);

            await using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(stream, cancellationToken);

            return storedFileName;
        }

        public Task<Stream> DownloadAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var storedFileName = NormalizeStoredFileName(fileName);
            var fullPath = Path.Combine(GetBasePath(), storedFileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("File not found.", storedFileName);
            }

            Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult(stream);
        }

        public Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var storedFileName = NormalizeStoredFileName(fileName);
            var fullPath = Path.Combine(GetBasePath(), storedFileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        public async Task StageChunkAsync(string fileName, int index, Stream content, CancellationToken cancellationToken = default)
        {
            var chunkDirectory = GetChunkDirectory(fileName);
            Directory.CreateDirectory(chunkDirectory);

            await using var stream = new FileStream(Path.Combine(chunkDirectory, index.ToString("D6")), FileMode.Create, FileAccess.Write, FileShare.None);
            await content.CopyToAsync(stream, cancellationToken);
        }

        public async Task<long> CommitChunksAsync(string fileName, int chunkCount, CancellationToken cancellationToken = default)
        {
            var chunkDirectory = GetChunkDirectory(fileName);
            var fullPath = Path.Combine(GetBasePath(), NormalizeStoredFileName(fileName));
            var chunkPaths = Enumerable.Range(0, chunkCount).Select(index => Path.Combine(chunkDirectory, index.ToString("D6"))).ToList();

            // Check first, so a missing part never leaves a partial file that looks complete.
            var missing = chunkPaths.FindIndex(chunkPath => !File.Exists(chunkPath));
            if (missing >= 0)
            {
                throw new InvalidOperationException($"Upload is missing part {missing + 1} of {chunkCount}.");
            }

            await using (var output = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                foreach (var chunkPath in chunkPaths)
                {
                    await using var chunk = new FileStream(chunkPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    await chunk.CopyToAsync(output, cancellationToken);
                }
            }

            Directory.Delete(chunkDirectory, recursive: true);
            return new FileInfo(fullPath).Length;
        }

        public Task<long?> GetSizeAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(GetBasePath(), NormalizeStoredFileName(fileName));
            return Task.FromResult(File.Exists(fullPath) ? new FileInfo(fullPath).Length : (long?)null);
        }

        public Task TrySetStatusTagAsync(string fileName, string status, CancellationToken cancellationToken = default) => Task.CompletedTask;

        private string GetChunkDirectory(string fileName) =>
            Path.Combine(GetBasePath(), ".chunks", NormalizeStoredFileName(fileName));

        private string GetBasePath()
        {
            if (string.IsNullOrWhiteSpace(_settings.BasePath))
            {
                throw new InvalidOperationException("FileStorage:BasePath is not configured.");
            }

            return _settings.BasePath;
        }

        private static string NormalizeStoredFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name is required.", nameof(fileName));
            }

            var candidate = fileName.Trim();
            if (Path.IsPathRooted(candidate))
            {
                candidate = Path.GetFileName(candidate);
            }

            return Path.GetFileName(candidate);
        }
    }
}

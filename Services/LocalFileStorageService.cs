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

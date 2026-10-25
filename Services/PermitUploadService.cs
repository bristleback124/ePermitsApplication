using System.Security.Cryptography;
using System.Text;
using ePermitsApp.DTOs;
using ePermitsApp.Helpers;
using ePermitsApp.Models;
using ePermitsApp.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace ePermitsApp.Services
{
    /// Uploads BP/CoO document files through the API in small chunks, so files of any size avoid
    /// the request size limit and the App Service request timeout. Completed uploads are attached
    /// to an application by reference (UploadedFiles on the create/update DTOs).
    public class PermitUploadService : IPermitUploadService
    {
        // Blob index tag values; an optional storage lifecycle rule can delete old "pending" uploads.
        public const string PendingStatus = "pending";
        public const string AttachedStatus = "attached";

        // Azure block blobs hold at most 50,000 blocks (~390 GB at 8 MB per chunk).
        public const int MaxChunks = 50_000;

        private readonly IFileStorageService _storage;
        private readonly UploadSettings _settings;
        private readonly byte[] _tokenKey;

        public PermitUploadService(IFileStorageService storage, IOptions<UploadSettings> settings, IConfiguration configuration)
        {
            _storage = storage;
            _settings = settings.Value;
            _tokenKey = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? string.Empty);
        }

        public bool IsEnabled => _settings.ChunkedUploads && _tokenKey.Length > 0;

        public StartUploadResponseDto Start(string fileName, string userId)
        {
            EnsureEnabled();

            var safeName = SanitizeFileName(fileName);
            if (string.IsNullOrWhiteSpace(safeName))
            {
                throw new InvalidOperationException("File name is required.");
            }

            // Same naming as IFileStorageService.UploadAsync, so downloads work unchanged.
            var path = $"{Guid.NewGuid()}_{safeName}";
            return new StartUploadResponseDto
            {
                Path = path,
                Token = Sign(path, userId),
                ChunkSizeBytes = UploadSettings.ChunkSizeBytes,
            };
        }

        public async Task StageChunkAsync(string path, string token, string userId, int index, Stream content, CancellationToken cancellationToken = default)
        {
            EnsureEnabled();
            EnsureValidToken(path, userId, token);

            if (index < 0 || index >= MaxChunks)
            {
                throw new InvalidOperationException($"Chunk index must be between 0 and {MaxChunks - 1}.");
            }

            await _storage.StageChunkAsync(path, index, content, cancellationToken);
        }

        public async Task<CompleteUploadResponseDto> CompleteAsync(CompleteUploadRequestDto request, string userId, CancellationToken cancellationToken = default)
        {
            EnsureEnabled();
            EnsureValidToken(request.Path, userId, request.Token);

            if (request.ChunkCount < 1 || request.ChunkCount > MaxChunks)
            {
                throw new InvalidOperationException($"Chunk count must be between 1 and {MaxChunks}.");
            }

            var size = await _storage.CommitChunksAsync(request.Path, request.ChunkCount, cancellationToken);
            await _storage.TrySetStatusTagAsync(request.Path, PendingStatus, cancellationToken);

            return new CompleteUploadResponseDto { Path = request.Path, Size = size };
        }

        public async Task<UploadedFileSet> ResolveAsync(
            IEnumerable<UploadedFileRefDto>? uploads,
            string? userId,
            IReadOnlyCollection<string> allowedSlots,
            CancellationToken cancellationToken = default)
        {
            var refs = uploads?.Where(upload => !string.IsNullOrWhiteSpace(upload.Path)).ToList() ?? new List<UploadedFileRefDto>();
            if (refs.Count == 0)
            {
                return UploadedFileSet.Empty;
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new InvalidOperationException("Uploaded files need a signed-in user.");
            }

            var bySlot = new Dictionary<string, List<FileMetadataDto>>();
            var sizeByPath = new Dictionary<string, long>(StringComparer.Ordinal);
            foreach (var upload in refs)
            {
                var slot = upload.Slot.Trim().ToLowerInvariant();
                if (!allowedSlots.Contains(slot))
                {
                    throw new InvalidOperationException($"Unknown document field for an uploaded file: {upload.Slot}.");
                }

                var path = upload.Path.Trim();
                if (!IsValidToken(path, userId, upload.Token))
                {
                    throw new InvalidOperationException($"Uploaded file {upload.Name} could not be verified. Please attach it again.");
                }

                if (!sizeByPath.TryGetValue(path, out var size))
                {
                    size = await _storage.GetSizeAsync(path, cancellationToken)
                        ?? throw new InvalidOperationException($"Uploaded file {upload.Name} was not found. Please attach it again.");
                    sizeByPath[path] = size;
                }

                if (!bySlot.TryGetValue(slot, out var files))
                {
                    files = new List<FileMetadataDto>();
                    bySlot[slot] = files;
                }

                if (files.All(file => file.Path != path))
                {
                    var name = SanitizeFileName(upload.Name);
                    files.Add(new FileMetadataDto
                    {
                        Name = string.IsNullOrWhiteSpace(name) ? NameFromPath(path) : name,
                        Size = size,
                        Path = path,
                    });
                }
            }

            return new UploadedFileSet(bySlot);
        }

        public async Task MarkAttachedAsync(IEnumerable<string> paths, CancellationToken cancellationToken = default)
        {
            foreach (var path in paths)
            {
                await _storage.TrySetStatusTagAsync(path, AttachedStatus, cancellationToken);
            }
        }

        private void EnsureEnabled()
        {
            if (!IsEnabled)
            {
                throw new InvalidOperationException("Chunked uploads are not enabled.");
            }
        }

        private void EnsureValidToken(string path, string userId, string? token)
        {
            if (!IsValidToken(path, userId, token))
            {
                throw new UnauthorizedAccessException("This upload doesn't belong to the current user.");
            }
        }

        private string Sign(string path, string userId)
        {
            using var hmac = new HMACSHA256(_tokenKey);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{path}|{userId}"));
            return Convert.ToBase64String(hash).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private bool IsValidToken(string path, string userId, string? token)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var expected = Encoding.UTF8.GetBytes(Sign(path.Trim(), userId));
            var actual = Encoding.UTF8.GetBytes(token.Trim());
            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }

        // FilePathHelper stores files as "name::size::path|...", so those characters can't appear in names.
        private static string SanitizeFileName(string? fileName) =>
            Path.GetFileName(fileName ?? string.Empty).Replace("|", "_").Replace(":", "_").Trim();

        private static string NameFromPath(string path)
        {
            var separator = path.IndexOf('_');
            return separator >= 0 ? path[(separator + 1)..] : path;
        }
    }
}

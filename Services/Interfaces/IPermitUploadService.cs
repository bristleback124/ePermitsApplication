using ePermitsApp.DTOs;
using ePermitsApp.Helpers;

namespace ePermitsApp.Services.Interfaces
{
    /// Chunked uploads of BP/CoO document files through the API, attached to applications by reference.
    public interface IPermitUploadService
    {
        /// True when the browser should upload files in chunks before Save/Submit.
        bool IsEnabled { get; }

        /// Reserves a stored file name for one upload; the token ties it to `userId`.
        StartUploadResponseDto Start(string fileName, string userId);

        Task StageChunkAsync(string path, string token, string userId, int index, Stream content, CancellationToken cancellationToken = default);

        Task<CompleteUploadResponseDto> CompleteAsync(CompleteUploadRequestDto request, string userId, CancellationToken cancellationToken = default);

        /// Checks each reference was issued to `userId`, targets an allowed slot and is a completed
        /// upload (size is read from storage), and groups them by slot.
        Task<UploadedFileSet> ResolveAsync(IEnumerable<UploadedFileRefDto>? uploads, string? userId, IReadOnlyCollection<string> allowedSlots, CancellationToken cancellationToken = default);

        /// Marks uploads as attached to an application, so a cleanup of abandoned uploads skips them.
        Task MarkAttachedAsync(IEnumerable<string> paths, CancellationToken cancellationToken = default);
    }
}

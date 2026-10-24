namespace ePermitsApp.Models;

public class UploadSettings
{
    public const long DefaultMaxPermitBytes = 500L * 1024 * 1024;

    // IIS request filtering in web.config caps requests at 2 GB; keep this value at or below it.
    public const long IisMaxAllowedContentLength = 2L * 1024 * 1024 * 1024;

    // Total size of one BP/CoO create or update request (all documents in one multipart body).
    public long MaxPermitBytes { get; set; } = DefaultMaxPermitBytes;

    // Size of each piece when the browser uploads a BP/CoO file in chunks through the API.
    // Every request stays small and quick, so files of any size avoid MaxPermitBytes and the
    // App Service request timeout. Keep well under the IIS default of ~28.6 MB.
    public const int ChunkSizeBytes = 8 * 1024 * 1024;

    // Upload BP/CoO files in chunks before Save/Submit (true), or send them inside the form (false).
    public bool ChunkedUploads { get; set; } = true;
}

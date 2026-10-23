namespace ePermitsApp.Models;

public class UploadSettings
{
    public const long DefaultMaxPermitBytes = 500L * 1024 * 1024;

    // IIS request filtering in web.config caps requests at 2 GB; keep this value at or below it.
    public const long IisMaxAllowedContentLength = 2L * 1024 * 1024 * 1024;

    // Total size of one BP/CoO create or update request (all documents in one multipart body).
    public long MaxPermitBytes { get; set; } = DefaultMaxPermitBytes;
}

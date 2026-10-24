namespace ePermitsApp.DTOs
{
    /// A file already uploaded in chunks through /api/uploads, attached to one document slot.
    public class UploadedFileRefDto
    {
        /// The document field it belongs to, as named in the create/update DTO
        /// (e.g. "appInfo.reqDocProofOwnership", "coOAppReqDoc.reqDocOthers").
        public string Slot { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    public class StartUploadRequestDto
    {
        public string FileName { get; set; } = string.Empty;
    }

    public class StartUploadResponseDto
    {
        public string Path { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public int ChunkSizeBytes { get; set; }
    }

    public class CompleteUploadRequestDto
    {
        public string Path { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public int ChunkCount { get; set; }
    }

    public class CompleteUploadResponseDto
    {
        public string Path { get; set; } = string.Empty;
        public long Size { get; set; }
    }
}

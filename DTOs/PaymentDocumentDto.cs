namespace ePermits.Models.DTOs
{
    public class PaymentDocumentDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int UploadedById { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

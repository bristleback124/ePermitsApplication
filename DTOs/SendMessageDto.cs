namespace ePermits.Models.DTOs
{
    public class SendMessageDto
    {
        public int ApplicationId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}

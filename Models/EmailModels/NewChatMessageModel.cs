namespace ePermitsApp.Models.EmailModels;

public class NewChatMessageModel
{
    public string RecipientName { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderRole { get; set; } = string.Empty;
    public string MessagePreview { get; set; } = string.Empty;
    public string ApplicationFormattedId { get; set; } = string.Empty;
    public string ApplicationType { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}

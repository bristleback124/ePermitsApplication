namespace ePermitsApp.Models;

public class EmailMessage
{
    public string To { get; set; } = string.Empty;
    public List<string> Cc { get; set; } = new();
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
}

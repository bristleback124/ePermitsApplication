namespace ePermitsApp.Models.EmailModels;

public class ApplicantCredentialsModel
{
    public string ApplicantName { get; set; } = string.Empty;
    public string EncoderName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string TempPassword { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}

namespace ePermitsApp.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendEmailAsync(string to, List<string> cc, string subject, string body, bool isHtml = true);
    Task SendTemplatedEmailAsync<TModel>(string to, string subject, string templateName, TModel model);
    Task SendTemplatedEmailAsync<TModel>(string to, List<string> cc, string subject, string templateName, TModel model);
}

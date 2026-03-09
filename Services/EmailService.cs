using System.Threading.Channels;
using ePermitsApp.Models;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services;

public class EmailService : IEmailService
{
    private readonly Channel<EmailMessage> _channel;
    private readonly IRazorViewRenderer _razorViewRenderer;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        Channel<EmailMessage> channel,
        IRazorViewRenderer razorViewRenderer,
        ILogger<EmailService> logger)
    {
        _channel = channel;
        _razorViewRenderer = razorViewRenderer;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        var email = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml
        };

        await _channel.Writer.WriteAsync(email);
        _logger.LogInformation("Email to {To} with subject '{Subject}' enqueued", to, subject);
    }

    public async Task SendTemplatedEmailAsync<TModel>(string to, string subject, string templateName, TModel model)
    {
        var body = await _razorViewRenderer.RenderViewToStringAsync(templateName, model);
        await SendEmailAsync(to, subject, body);
    }
}

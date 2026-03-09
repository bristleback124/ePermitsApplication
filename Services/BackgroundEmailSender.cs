using System.Threading.Channels;
using ePermitsApp.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ePermitsApp.Services;

public class BackgroundEmailSender : BackgroundService
{
    private readonly Channel<EmailMessage> _channel;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<BackgroundEmailSender> _logger;

    public BackgroundEmailSender(
        Channel<EmailMessage> channel,
        IOptions<EmailSettings> emailSettings,
        ILogger<BackgroundEmailSender> logger)
    {
        _channel = channel;
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BackgroundEmailSender started");

        await foreach (var email in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(MailboxAddress.Parse(email.To));
                message.Subject = email.Subject;
                message.Body = new TextPart(email.IsHtml ? "html" : "plain")
                {
                    Text = email.Body
                };

                using var client = new SmtpClient();
                var secureSocketOptions = _emailSettings.EnableSsl
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.None;

                await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, secureSocketOptions, stoppingToken);
                await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword, stoppingToken);
                await client.SendAsync(message, stoppingToken);
                await client.DisconnectAsync(true, stoppingToken);

                _logger.LogInformation("Email sent to {To} with subject '{Subject}'", email.To, email.Subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To} with subject '{Subject}'", email.To, email.Subject);
            }
        }

        _logger.LogInformation("BackgroundEmailSender stopped");
    }
}

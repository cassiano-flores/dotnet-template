using DotnetTemplate.Exceptions;
using DotnetTemplate.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace DotnetTemplate.Services;

public sealed class EmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task Send(string to, string subject, string body, bool isBodyHtml, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.Password))
        {
            _logger.LogError("O envio de e-mail não está configurado: senha SMTP ausente.");

            throw new ApiException(
                StatusCodes.Status503ServiceUnavailable,
                "O serviço de e-mail está temporariamente indisponível.");
        }

        using var message = new MailMessage
        {
            From = new MailAddress(
                _options.From,
                _options.FromName),

            Subject = subject,
            Body = body,
            IsBodyHtml = isBodyHtml
        };

        message.To.Add(to);

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.UseSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(
                _options.Username,
                _options.Password)
        };

        cancellationToken.ThrowIfCancellationRequested();

        await client.SendMailAsync(
            message,
            cancellationToken);
    }
}

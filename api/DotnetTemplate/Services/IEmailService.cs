namespace DotnetTemplate.Services;

public interface IEmailService
{
    Task Send(string to, string subject, string body, bool isBodyHtml, CancellationToken cancellationToken);
}

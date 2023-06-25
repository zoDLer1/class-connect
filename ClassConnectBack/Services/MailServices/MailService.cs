using System.Text;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace ClassConnect.Services.MailServices;

public class MailService : IMailService
{
    private IOptions<EmailSettings> _settings;
    private SmtpClient _client;

    public MailService(IOptions<EmailSettings> settings)
    {
        _settings = settings;
        _client = new SmtpClient(settings.Value.Host, settings.Value.Port);
        _client.UseDefaultCredentials = false;
        _client.Credentials = new NetworkCredential(settings.Value.Sender, settings.Value.Password);
        _client.DeliveryMethod = SmtpDeliveryMethod.Network;
        _client.EnableSsl = true;
    }

    public void SendMail(string recipient, string subject, string body)
    {
        var message = new MailMessage(_settings.Value.Sender, recipient);
        message.Subject = subject;
        message.Body = body;
        message.BodyEncoding = Encoding.UTF8;
        message.IsBodyHtml = true;
        _client.Send(message);
    }
}

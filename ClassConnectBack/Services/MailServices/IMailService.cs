using ClassConnect.Services.MailServices.Templates;

namespace ClassConnect.Services.MailServices;

/// <summary>
/// Сервис отправки рассылки
/// </summary>
public interface IMailService
{
    void SendMail(string recipient, string subject, string text);
    void SendMail(string recipient, IMail mail);
}

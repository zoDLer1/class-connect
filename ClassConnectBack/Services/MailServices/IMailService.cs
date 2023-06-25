namespace ClassConnect.Services.MailServices;

/// <summary>
/// Сервис отправки рассылки
/// </summary>
public interface IMailService
{
    public void SendMail(string recipient, string subject, string text);
}

namespace ClassConnect.Services.MailServices.Templates;

public class Mail : IMail
{
    public string Subject { get; }
    public string Body { get; }

    public Mail(string subject, string body)
    {
        Subject = subject;
        Body = body;
    }
}

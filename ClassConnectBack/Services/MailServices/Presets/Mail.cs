namespace ClassConnect.Services.MailServices.Presets;

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

namespace ClassConnect.Services.MailServices.Templates;

public interface IMail
{
    public string Subject { get; }
    public string Body { get; }
}

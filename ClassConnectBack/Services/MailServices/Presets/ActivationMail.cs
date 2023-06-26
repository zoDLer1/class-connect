namespace ClassConnect.Services.MailServices.Presets;

public class ActivationMail : Mail
{
    public ActivationMail(string link)
        : base(
            "Активация аккаунта ClassConnect",
            $"<div><h1>Для активации аккаунта перейдите по ссылке </h1><a href=\"{link}\">{link}</a></div>"
        ) { }
}

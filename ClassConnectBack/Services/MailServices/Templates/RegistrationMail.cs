namespace ClassConnect.Services.MailServices.Templates;

public class RegistrationMail : Mail
{
    public RegistrationMail(string link, string login, string password)
        : base(
            "Регистрация на ClassConnect",
            $"<div><h1>Для активации аккаунта перейдите по ссылке </h1><a href=\"{link}\">{link}</a><p><strong>Логин:</strong>{login}</p><p><strong>Пароль:</strong>{password}</p></div>"
        ) { }
}

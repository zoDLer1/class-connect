using ClassConnect.Exceptions;
using ClassConnect.Helpers;
using ClassConnect.Models;
using ClassConnect.Services.MailServices;
using ClassConnect.Services.MailServices.Templates;
using ClassConnect.Services.UserServices;

namespace ClassConnect.Services.AuthenticationServices;

public class AuthenticationService : IAuthenticationService
{
    private GenerateLink _linkGenerator;
    private IMailService _mailService;
    private IUserService _userService;

    public AuthenticationService(
        GenerateLink linkGenerator,
        IMailService mailService,
        IUserService userService
    )
    {
        _linkGenerator = linkGenerator;
        _mailService = mailService;
        _userService = userService;
    }

    public async Task<User> LoginAsync(string email, string password)
    {
        var user = await _userService.GetByEmailAsync(email);
        if (user is null)
            throw new UserNotFoundException() { PropertyName = "Email" };

        if (BCrypt.Net.BCrypt.Verify(password, user.Password))
            return user;
        throw new InvalidPasswordException() { PropertyName = "Password" };
    }

    public async Task RegisterAsync(
        string name,
        string surname,
        string? patronymic,
        string email,
        string password,
        UserRole roleId
    )
    {
        var hasPassword = !string.IsNullOrWhiteSpace(password);
        if (!hasPassword)
            password = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 10);

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User()
        {
            Name = name,
            Surname = surname,
            Patronymic = patronymic,
            Email = email,
            Password = hashedPassword,
            RegTime = DateTime.Now,
            RoleId = roleId,
            ActivationLink = Guid.NewGuid().ToString()
        };

        await _userService.CreateAsync(user);

        var link = _linkGenerator("user/activate/" + user.ActivationLink);
        _mailService.SendMail(
            user.Email,
            hasPassword ? new ActivationMail(link) : new RegistrationMail(link, user.Email, password)
        );
    }
}

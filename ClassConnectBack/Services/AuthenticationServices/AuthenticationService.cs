using ClassConnect.Exceptions;
using ClassConnect.Models;
using ClassConnect.Services.MailServices;
using ClassConnect.Services.UserServices;

namespace ClassConnect.Services.AuthenticationServices;

public class AuthenticationService : IAuthenticationService
{
    private IMailService _mailService;
    private IUserService _userService;

    public AuthenticationService(IMailService mailService, IUserService userService)
    {
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
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User()
        {
            Name = name,
            Surname = surname,
            Patronymic = patronymic,
            Email = email,
            Password = hashedPassword,
            RegTime = DateTime.Now,
            RoleId = roleId
        };

        await _userService.CreateAsync(user);
    }
}

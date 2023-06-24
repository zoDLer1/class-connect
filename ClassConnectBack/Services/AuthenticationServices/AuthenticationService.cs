using ClassConnect.Exceptions;
using ClassConnect.Models;
using ClassConnect.Services.UserServices;

namespace ClassConnect.Services.AuthenticationServices;

public class AuthenticationService : IAuthenticationService
{
    private IUserService _userService;

    public AuthenticationService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<User> LoginAsync(string email, string password)
    {
        var user = await _userService.GetByEmailAsync(email);
        if (user is null)
            throw new UserNotFoundException();

        if (BCrypt.Net.BCrypt.Verify(password, user.Password))
            return user;
        throw new InvalidPasswordException();
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

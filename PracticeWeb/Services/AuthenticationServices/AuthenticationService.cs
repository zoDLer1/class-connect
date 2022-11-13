using PracticeWeb.Exceptions;
using PracticeWeb.Models;
using PracticeWeb.Services.UserServices;

namespace PracticeWeb.Services.AuthenticationServices;

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
        string firstName, 
        string lastName, 
        string? patronymic,
        string email, 
        string password,
        int roleId)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User() {
            FirstName = firstName,
            LastName = lastName,
            Patronymic = patronymic,
            Email = email,
            Password = hashedPassword,
            RegTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
            RoleId = roleId
        };
        
        await _userService.CreateAsync(user);
    }
}
using ClassConnect.Models;
using ClassConnect.Services.UserServices;

namespace ClassConnect.Services.AuthenticationServices;

/// <summary>
/// Сервис аутентификации пользователей
/// </summary>
public interface IAuthenticationService
{
    Task<User> LoginAsync(string email, string password);
    Task RegisterAsync(
        string firstName,
        string lastName,
        string? patronymic,
        string email,
        string password,
        UserRole role
    );
}

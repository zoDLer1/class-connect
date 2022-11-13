using PracticeWeb.Models;

namespace PracticeWeb.Services.AuthenticationServices;

/// <summary>
/// Сервис авторизации пользователей
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
        int roleId);
}
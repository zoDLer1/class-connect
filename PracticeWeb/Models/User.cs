namespace PracticeWeb.Models;

public class User : IntegerIdCommonModel
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Patronymic { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public long RegTime { get; set; }

    public UserRole RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public string? RefreshTokenId { get; set; }
    public RefreshToken? RefreshToken { get; set; }
}
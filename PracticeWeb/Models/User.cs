using System.ComponentModel.DataAnnotations;

namespace PracticeWeb.Models;

public class User : CommonModel<int>
{
    [StringLength(30)]
    public string Name { get; set; } = null!;

    [StringLength(45)]
    public string Surname { get; set; } = null!;

    [StringLength(45)]
    public string? Patronymic { get; set; }

    [StringLength(50)]
    public string Email { get; set; } = null!;

    [StringLength(70)]
    public string Password { get; set; } = null!;

    public DateTime RegTime { get; set; }

    public UserRole RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public string? RefreshTokenId { get; set; }
    public RefreshToken? RefreshToken { get; set; }
}

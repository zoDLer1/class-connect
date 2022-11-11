namespace PracticeWeb.Models;

public class User : IntegerIdCommonModel
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
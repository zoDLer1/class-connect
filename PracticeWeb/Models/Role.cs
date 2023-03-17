namespace PracticeWeb.Models;

public class Role
{
    public UserRole Id { get; set; }
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
}
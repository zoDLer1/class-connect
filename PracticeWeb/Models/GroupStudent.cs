namespace PracticeWeb.Models;

public class GroupStudent
{
    public string GroupId { get; set; } = null!;
    public Group Group { get; set; } = null!;

    public int StudentId { get; set; }
    public User Student { get; set; } = null!;
}
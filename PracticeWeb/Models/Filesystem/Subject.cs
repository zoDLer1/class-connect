namespace PracticeWeb.Models;

public class Subject : StringIdCommonModel
{
    public string GroupId { get; set; } = null!;
    public Group Group { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
namespace PracticeWeb.Models;

public class Group : StringIdCommonModel
{
    public string Name { get; set; } = null!;
    public int TeacherId { get; set; }
    public User Teacher { get; set; } = null!;
}
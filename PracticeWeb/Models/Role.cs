namespace PracticeWeb.Models;

public class Role : IntegerIdCommonModel
{
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
}
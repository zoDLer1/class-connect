namespace PracticeWeb.Models;

public class Connection
{
    public string ParentId { get; set; } = null!;
    public Item Parent { get; set; } = null!;

    public string ChildId { get; set; } = null!;
    public Item Child { get; set; } = null!;
}

namespace PracticeWeb.Models;

public class WorkItem
{
    public string WorkId { get; set; } = null!;
    public Work Work { get; set; } = null!;

    public string ItemId { get; set; } = null!;
    public Item Item { get; set; } = null!;
}
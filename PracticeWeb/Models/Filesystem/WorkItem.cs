namespace PracticeWeb.Models;

public class WorkItem : CommonModel<string>
{
    public string WorkId { get; set; } = null!;
    public Work Work { get; set; } = null!;
}

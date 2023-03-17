namespace PracticeWeb.Models;

public class WorkItem : StringIdCommonModel
{
    public string WorkId { get; set; } = null!;
    public Work Work { get; set; } = null!;
}

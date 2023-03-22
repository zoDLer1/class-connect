namespace PracticeWeb.Models;

public class TaskEntity : CommonModel<string>
{
    public DateTime? Until { get; set; }
    public Item Item { get; set; } = null!;
}

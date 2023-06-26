namespace ClassConnect.Models;

public class TaskEntity : CommonModel<string>
{
    public DateTime? Until { get; set; }
    public ItemEntity Item { get; set; } = null!;
}

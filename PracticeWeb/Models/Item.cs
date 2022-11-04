namespace PracticeWeb.Models;

public class Item : CommonModel
{
    public int TypeId { get; set; }
    public ItemType Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime CreationTime { get; set; }
}
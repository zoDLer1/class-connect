namespace PracticeWeb.Models;

public class Item : StringIdCommonModel
{
    public Type TypeId { get; set; }
    public ItemType Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime CreationTime { get; set; }
    
    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;
}
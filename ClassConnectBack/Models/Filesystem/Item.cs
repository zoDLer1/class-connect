using System.ComponentModel.DataAnnotations;

namespace ClassConnect.Models;

public class Item : CommonModel<string>
{
    public Type TypeId { get; set; }
    public ItemType Type { get; set; } = null!;

    [StringLength(100)]
    public string Name { get; set; } = null!;
    public DateTime CreationTime { get; set; }

    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;
}

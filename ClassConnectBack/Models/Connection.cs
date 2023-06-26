namespace ClassConnect.Models;

public class Connection
{
    public string ParentId { get; set; } = null!;
    public ItemEntity Parent { get; set; } = null!;

    public string ChildId { get; set; } = null!;
    public ItemEntity Child { get; set; } = null!;
}

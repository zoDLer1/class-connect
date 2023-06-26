namespace ClassConnect.Models;

public class Group : CommonModel<string>
{
    public int TeacherId { get; set; }
    public User Teacher { get; set; } = null!;
    public ItemEntity Item { get; set; } = null!;
}

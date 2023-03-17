namespace PracticeWeb.Models;

public class Access {
    public Permission Permission { get; set; }

    public string ItemId { get; set; } = null!;
    public Item Item { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
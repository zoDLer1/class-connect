using System.ComponentModel.DataAnnotations;

namespace PracticeWeb.Models;

public class Group : CommonModel<string>
{
    public int TeacherId { get; set; }
    public User Teacher { get; set; } = null!;
    public Item Item { get; set; } = null!;
}

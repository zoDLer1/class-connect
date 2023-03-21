using System.ComponentModel.DataAnnotations;

namespace PracticeWeb.Models;

public class Group : StringIdCommonModel
{
    public int TeacherId { get; set; }
    public User Teacher { get; set; } = null!;
}

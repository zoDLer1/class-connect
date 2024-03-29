using System.ComponentModel.DataAnnotations;

namespace ClassConnect.Models;

public class Role
{
    public UserRole Id { get; set; }

    [StringLength(20)]
    public string Name { get; set; } = null!;

    [StringLength(20)]
    public string Title { get; set; } = null!;
}

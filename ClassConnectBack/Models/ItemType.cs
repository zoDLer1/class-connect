using System.ComponentModel.DataAnnotations;

namespace ClassConnect.Models;

public class ItemType
{
    public Type Id { get; set; }

    [StringLength(10)]
    public string Name { get; set; } = null!;
}

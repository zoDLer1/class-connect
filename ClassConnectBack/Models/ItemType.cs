using System.ComponentModel.DataAnnotations;
using ClassConnect.Services.FileSystemServices;

namespace ClassConnect.Models;

public class ItemType
{
    public Item Id { get; set; }

    [StringLength(10)]
    public string Name { get; set; } = null!;
}

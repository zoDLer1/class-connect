using System.ComponentModel.DataAnnotations;

namespace ClassConnect.Controllers.Models;

public class FolderModel : ItemModel
{
    [Required(ErrorMessage = "Укажите имя объекта")]
    [StringLength(70, ErrorMessage = "Используйте менее 70 символов")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Укажите тип объекта")]
    [EnumDataType(typeof(Item), ErrorMessage = "Некорректное значение параметра Type")]
    public string Type { get; set; } = null!;

    public int? TeacherId { get; set; }

    [StringLength(300, ErrorMessage = "Используйте менее 300 символов")]
    public string? Description { get; set; }
    public DateTime? Until { get; set; }
}

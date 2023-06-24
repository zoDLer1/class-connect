using System.ComponentModel.DataAnnotations;

namespace ClassConnect.Controllers.Models;

public class NewNameItemModel : ItemModel
{
    [Required(ErrorMessage = "Укажите имя объекта")]
    [StringLength(100, ErrorMessage = "Используйте менее 100 символов")]
    public string Name { get; set; } = null!;
}

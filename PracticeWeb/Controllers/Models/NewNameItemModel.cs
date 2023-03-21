using System.ComponentModel.DataAnnotations;

namespace PracticeWeb.Controllers.Models;

public class NewNameItemModel : ItemModel
{
    [Required(ErrorMessage = "Укажите имя объекта")]
    [StringLength(70, ErrorMessage = "Используйте менее 70 символов")]
    public string Name { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;

namespace PracticeWeb.Controllers.Models;

public class ItemModel
{
    [Required(ErrorMessage = "Укажите Id объекта")]
    [StringLength(36, MinimumLength = 36, ErrorMessage = "Некорректный Id объекта")]
    public string Id { get; set; } = string.Empty;
}

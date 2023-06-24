using System.ComponentModel.DataAnnotations;

namespace ClassConnect.Controllers.Models;

public class NewTypeItemModel : ItemModel
{
    [Required(ErrorMessage = "Укажите тип объекта")]
    [EnumDataType(typeof(Type), ErrorMessage = "Некорректное значение параметра Type")]
    public string Type { get; set; } = null!;
}

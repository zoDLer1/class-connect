using System.ComponentModel.DataAnnotations;
using ClassConnect.Services.UserServices;

namespace ClassConnect.Controllers.Models;

public class UserModel : SignupModel
{
    [Required(ErrorMessage = "Укажите роль пользователя")]
    [EnumDataType(typeof(UserRole), ErrorMessage = "Некорректное значение параметра Role")]
    public string Role { get; set; } = null!;
    public new string Password { get; set; } = string.Empty;
    public new string PasswordConfirm { get; set; } = string.Empty;
}

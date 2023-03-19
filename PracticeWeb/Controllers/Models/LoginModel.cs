using System.ComponentModel.DataAnnotations;

namespace PracticeWeb.Controllers.Models;

public class LoginModel {
    [Required(ErrorMessage = "Укажите email")]
    [StringLength(50, ErrorMessage = "Используйте менее 50 символов")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Укажите пароль")]
    [StringLength(70, ErrorMessage = "Используйте менее 70 символов")]
    public string Password { get; set; } = string.Empty;
}

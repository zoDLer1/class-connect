using System.ComponentModel.DataAnnotations;

namespace PracticeWeb.Controllers.Models;

public class SignupModel {
    [Required(ErrorMessage = "Укажите имя")]
    [StringLength(30, ErrorMessage = "Используйте менее 30 символов")]
    [MinLength(2, ErrorMessage = "Используйте не менее 2 символов")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Укажите фамилию")]
    [StringLength(45, ErrorMessage = "Используйте менее 45 символов")]
    [MinLength(2, ErrorMessage = "Используйте не менее 2 символов")]
    public string Surname { get; set; } = string.Empty;
    [StringLength(45, ErrorMessage = "Используйте менее 45 символов")]
    public string Patronymic { get; set; } = string.Empty;
    [Required(ErrorMessage = "Укажите email")]
    [StringLength(50, ErrorMessage = "Используйте менее 50 символов")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Укажите пароль")]
    [StringLength(70, ErrorMessage = "Используйте менее 70 символов")]
    [MinLength(8, ErrorMessage = "Используйте не менее 8 символов")]
    public string Password { get; set; } = string.Empty;
    [Required(ErrorMessage = "Повторите пароль")]
    [StringLength(70, ErrorMessage = "Используйте менее 70 символов")]
    [MinLength(8, ErrorMessage = "Используйте не менее 8 символов")]
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    public string PasswordConfirm { get; set; } = string.Empty;
}

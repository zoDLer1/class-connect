using System.ComponentModel.DataAnnotations;

namespace ClassConnect.Controllers.Models;

public class StudentModel : ItemModel
{
    [Required(ErrorMessage = "Укажите Id студента")]
    public int StudentId { get; set; }
}

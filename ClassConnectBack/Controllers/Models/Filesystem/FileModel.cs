using System.ComponentModel.DataAnnotations;

namespace ClassConnect.Controllers.Models;

public class FileModel : ItemModel
{
    [Required(ErrorMessage = "Передайте файл")]
    public IFormFile UploadedFile { get; set; } = null!;
}

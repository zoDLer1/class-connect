namespace PracticeWeb.Models;

public class FileEntity : CommonModel
{
    public string Format { get; set; } = null!;
    public string MimeType { get; set; } = null!;
}
namespace PracticeWeb.Models;

public class FileEntity : CommonModel
{
    public string Extension { get; set; } = null!;
    public string MimeType { get; set; } = null!;
}
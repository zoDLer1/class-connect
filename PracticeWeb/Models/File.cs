namespace PracticeWeb.Models;

public class FileEntity : StringIdCommonModel
{
    public string Extension { get; set; } = null!;
    public string MimeType { get; set; } = null!;
}
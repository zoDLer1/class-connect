namespace ClassConnect.Models;

public class FileEntity : CommonModel<string>
{
    public string Extension { get; set; } = null!;
    public string MimeType { get; set; } = null!;
    public Item Item { get; set; } = null!;
}

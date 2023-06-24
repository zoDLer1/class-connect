using ClassConnect.Models;

namespace ClassConnect;

public class FolderItem
{
    public string Name { get; set; } = null!;
    public string Guid { get; set; } = null!;

    public ItemType Type { get; set; } = null!;
    public string? MimeType { get; set; }

    public FolderData Data { get; set; } = null!;
}

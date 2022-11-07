using PracticeWeb.Models;

namespace PracticeWeb;

public class FolderItem 
{
    public string Name { get; set; } = null!;
    public List<string> Path { get; set; } = null!;
    public string Guid { get; set; } = null!;

    public ItemType Type { get; set; } = null!;
    public string? MimeType { get; set; }

    public DateTime CreationTime { get; set; }
    public string CreatorName { get; set; } = null!;
}
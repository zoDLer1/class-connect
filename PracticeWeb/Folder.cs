using PracticeWeb.Models;

namespace PracticeWeb;

public class Folder
{
    public string Name { get; set; } = null!;
    public ItemType Type { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string RealPath { get; set; } = null!;
    public string Guid { get; set; } = null!;

    public int ItemsCount { get => Items.Count(); }
    public IEnumerable<FolderItem> Items { get; set; } = null!;

    public DateTime CreationTime { get; set; }
    public string CreatorName { get; set; } = null!;
}

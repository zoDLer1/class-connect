namespace PracticeWeb;

public enum ItemTypes 
{
    Folder,
    File
}

public class Item 
{
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string Guid { get; set; } = null!;

    public ItemTypes Type { get; set; }
    public string? MimeType { get; set; }

    public DateTime CreationTime { get; set; }
    public string CreatorName { get; set; } = null!;
}
namespace PracticeWeb;

public enum ItemTypes 
{
    Folder,
    File
}

public class Item 
{
    public string Name { get; set; } = null!;
    public ItemTypes Type { get; set; }
    public DateTime CreationTime { get; set; }
}
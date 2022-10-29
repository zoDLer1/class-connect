namespace PracticeWeb;

public class Folder
{
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;

    public IEnumerable<Item> Items { get; set; } = null!;
    public int ItemsCount { get => Items.Count(); }
}

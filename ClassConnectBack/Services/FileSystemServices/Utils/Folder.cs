using ClassConnect.Models;

namespace ClassConnect.Services.FileSystemServices;

public class Folder
{
    public string Name { get; set; } = null!;
    public ItemType Type { get; set; } = null!;
    public List<object> Path { get; set; } = null!;
    public string Guid { get; set; } = null!;

    public IEnumerable<object>? Children { get; set; } = null!;
    public FolderData Data { get; set; } = null!;
    public List<string> Access { get; set; } = null!;
    public Boolean IsEditable { get; set; }
}

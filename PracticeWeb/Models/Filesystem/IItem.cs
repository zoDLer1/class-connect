namespace PracticeWeb.Models;

public interface IItem : CommonModel<string>
{
    int TypeId { get; set; }
    ItemType Type { get; set; }
    string Name { get; set; }
    DateTime CreationTime { get; set; }
}
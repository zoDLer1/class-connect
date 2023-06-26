namespace ClassConnect.Services.FileSystemServices;

public static class TypeDependence
{
    public static HashSet<Item> File
    {
        get { return new HashSet<Item> { Item.Subject, Item.Folder, Item.Task, Item.Work }; }
    }
    public static HashSet<Item> Folder
    {
        get { return new HashSet<Item> { Item.Subject, Item.Folder, Item.Task }; }
    }
    public static HashSet<Item> Subject
    {
        get { return new HashSet<Item> { Item.Group }; }
    }
    public static HashSet<Item> Group
    {
        get { return new HashSet<Item> { Item.Folder }; }
    }
    public static HashSet<Item> Work
    {
        get { return new HashSet<Item> { Item.Task }; }
    }
    public static HashSet<Item> Task
    {
        get { return new HashSet<Item> { Item.Subject, Item.Folder, Item.Task }; }
    }
}

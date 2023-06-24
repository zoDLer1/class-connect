namespace PracticeWeb;

public enum Type
{
    File,
    Folder,
    Subject,
    Group,
    Work,
    Task,
}

public static class TypeDependence
{
    public static HashSet<Type> File
    {
        get { return new HashSet<Type> { Type.Subject, Type.Folder, Type.Task, Type.Work }; }
    }
    public static HashSet<Type> Folder
    {
        get { return new HashSet<Type> { Type.Subject, Type.Folder, Type.Task }; }
    }
    public static HashSet<Type> Subject
    {
        get { return new HashSet<Type> { Type.Group }; }
    }
    public static HashSet<Type> Group
    {
        get { return new HashSet<Type> { Type.Folder }; }
    }
    public static HashSet<Type> Work
    {
        get { return new HashSet<Type> { Type.Task }; }
    }
    public static HashSet<Type> Task
    {
        get { return new HashSet<Type> { Type.Subject, Type.Folder, Type.Task }; }
    }
}

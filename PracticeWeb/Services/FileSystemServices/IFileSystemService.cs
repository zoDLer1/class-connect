using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices;

public interface IFileSystemService
{
    public string? RootGuid { get; }
    Task RecreateFileSystemAsync();
    Task<object> SubmitWork(string id, User user);
    Task<object> MarkWork(string id, int? mark, User user);
    Task<object> GetObjectAsync(string id, User user);
    Task<object> CreateWorkAsync(string parentId, string name, IFormFile file, User user);
    Task<object> CreateFileAsync(string parentId, IFormFile file, User user);
    Task<object> CreateFolderAsync(
        string parentId,
        string name,
        Type type,
        User user,
        Dictionary<string, object>? parameters
    );
    Task RenameAsync(string id, string newName, User user);
    Task UpdateTypeAsync(string id, Type newType, User user);
    Task RemoveAsync(string id, User user);
}

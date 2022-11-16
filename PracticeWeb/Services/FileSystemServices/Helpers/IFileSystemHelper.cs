using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public interface IFileSystemHelper
{
    Task<List<string>> HasAccessAsync(string id, User user, List<string> path);
    Task<Object> GetAsync(string id, User user);
    Task<Object> GetChildItemAsync(string id, User user);
    Task<(string, Object)> CreateAsync(string parentId, string name, User user, Dictionary<string, string>? parameters=null);
    Task<FolderItem> UpdateAsync(string id, string newName);
    Task<FolderItem> UpdateTypeAsync(string id, string newType);
    Task DeleteAsync(string id);
}
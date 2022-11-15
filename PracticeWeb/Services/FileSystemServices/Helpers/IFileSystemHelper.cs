using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public interface IFileSystemHelper
{
    Task<FolderItem> GetAsync(string id);
    Task<(string, FolderItem)> CreateAsync(string parentId, string name, Dictionary<string, string>? parameters=null);
    Task<FolderItem> UpdateAsync(string id, string newName);
    Task<FolderItem> UpdateTypeAsync(string id, string newType);
    Task DeleteAsync(string id);
}
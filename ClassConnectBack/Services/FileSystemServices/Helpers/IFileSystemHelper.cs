using ClassConnect.Models;

namespace ClassConnect.Services.FileSystemServices.Helpers;

public interface IFileSystemHelper
{
    Task<ItemAccess> HasAccessAsync(string id, User user, List<string> path);
    Task CheckIfCanCreateAsync(string parentId, User user);
    Task<object> GetAsync(string id, User user, Boolean asChild);
    Task<(string, object)> CreateAsync(
        string parentId,
        string name,
        User user,
        Dictionary<string, object>? parameters = null
    );
    Task<FolderItem> UpdateAsync(string id, string newName, User user);
    Task<FolderItem> UpdateTypeAsync(string id, Item newType, User user);
    Task DeleteAsync(string id, User user);
}

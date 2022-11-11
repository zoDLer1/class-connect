using PracticeWeb.Models;

namespace PracticeWeb.Services.ItemStorageServices;

public interface IItemStorageService : IDataService<string, Item>
{
    Task<ItemType?> GetItemTypeAsync(int id);
    Task CreateFileAsync(FileEntity entity);
    Task<FileEntity?> GetFileAsync(string id);
    Task DeleteFileAsync(string id);
    Task CreateConnectionAsync(Connection entity);
    Task<List<Connection>> GetConnectionsByParentAsync(string parentId);
    Task<Connection?> GetConnectionByChildAsync(string childId);
    Task DeleteConnectionAsync(string parentId, string childId);
}
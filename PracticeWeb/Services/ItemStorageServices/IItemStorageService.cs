using PracticeWeb.Models;

namespace PracticeWeb.Services.ItemStorageServices;

public interface IItemStorageService : IDataService<Item>
{
    Task<ItemType?> GetItemTypeAsync(int id);
    Task CreateConnectionAsync(Connection connection);
    Task<List<Connection>> GetConnectionsByParentAsync(string parentId);
    Task<Connection?> GetConnectionByChildAsync(string childId);
    Task DeleteConnectionAsync(string parentId, string childId);
}
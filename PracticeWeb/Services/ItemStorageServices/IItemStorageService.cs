using PracticeWeb.Models;

namespace PracticeWeb.Services.ItemStorageServices;

public interface IItemStorageService : IDataService<Item>
{
    Task<ItemType?> GetItemTypeAsync(int id);
    Task CreateConnectionAsync(Connection connection);
    Task GetConnectionsByParentAsync(string parentId);
    Task GetConnectionByChildAsync(string childId);
    Task DeleteConnectionAsync(string parentId, string childId);
}
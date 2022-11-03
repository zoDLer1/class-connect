using PracticeWeb.Models;

namespace PracticeWeb.Services.ItemStorageServices;

public interface IItemStorageService : IDataService<Item>
{
    Task<ItemType?> GetItemType(int id);
}
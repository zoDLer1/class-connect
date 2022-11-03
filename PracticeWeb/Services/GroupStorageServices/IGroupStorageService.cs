using PracticeWeb.Models;

namespace PracticeWeb.Services.GroupStorageServices;

public interface IGroupStorageService : IDataService<Group>
{
    Task<Group?> GetByGroupNameAsync(string groupName);
}
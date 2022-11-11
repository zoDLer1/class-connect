using PracticeWeb.Models;

namespace PracticeWeb.Services.GroupStorageServices;

public interface IGroupStorageService : IDataService<string, Group>
{
    Task<Group?> GetByGroupNameAsync(string groupName);
}
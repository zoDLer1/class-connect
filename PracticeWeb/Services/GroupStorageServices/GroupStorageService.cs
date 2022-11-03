using PracticeWeb.Models;

namespace PracticeWeb.Services.GroupStorageServices;

public class GroupStorageService : IGroupStorageService
{
    public async Task<Group> CreateAsync(Group entity)
    {
        throw new NotImplementedException();
    }

    public async Task<Group?> GetAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Group?> GetByGroupNameAsync(string groupName)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Group>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(string id, Group entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }
}
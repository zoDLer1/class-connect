using Microsoft.EntityFrameworkCore;
using PracticeWeb.Models;

namespace PracticeWeb.Services.GroupStorageServices;

public class GroupStorageService : IGroupStorageService
{
    private Context _context;
    private CommonQueries<string, Group> _common;

    public GroupStorageService(Context context)
    {
        _context = context;
        _common = new CommonQueries<string, Group>(_context);
    }

    public async Task<Group> CreateAsync(Group entity) =>
        await _common.CreateAsync(entity);

    public async Task<Group?> GetAsync(string id) =>
        await _common.GetAsync(id, _context.Groups);

    public async Task<Group?> GetByGroupNameAsync(string groupName) =>
        await _context.Groups.FirstOrDefaultAsync(g => g.Name == groupName);

    public async Task<List<Group>> GetAllAsync() =>
        await _common.GetAllAsync(_context.Groups);

    public async Task UpdateAsync(Group entity) =>
        await _common.UpdateAsync(entity);

    public async Task DeleteAsync(string id) =>
        await _common.DeleteAsync(id);
}
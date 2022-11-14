using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public class GroupHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, Group> _commonGroupQueries;
    
    public GroupHelperService(IHostEnvironment env, Context context) : base(env, context)
    {
        _commonGroupQueries = new CommonQueries<string, Group>(_context);
    }

    public async Task<(string, FolderItem)> CreateAsync(string parentId, string name, Dictionary<string, string>? parameters=null)
    {
        // Проверяем, является ли родитель корнем
        if (await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == parentId) != null)
            throw new InvalidPathException();

        if (await _context.Groups.FirstOrDefaultAsync(g => g.Name == name) != null)
            throw new InvalidGroupNameException();
        
        var (path, item) = await base.CreateAsync(parentId, name, 2);
        var group = new Group
        {
            Id = item.Guid,
            Name = name
        };
        await _commonGroupQueries.CreateAsync(group);
        return (path, item);
    }

    public async override Task<FolderItem> UpdateAsync(string id, string newName)
    {
        var group = await _commonGroupQueries.GetAsync(id, _context.Groups);
        if (group == null)
            throw new ItemNotFoundException();

        var anotherGroup = await _context.Groups.FirstOrDefaultAsync(s => s.Name == newName);
        if (anotherGroup != null)
            throw new InvalidGroupNameException();

        var item = await base.UpdateAsync(id, newName);
        group.Name = newName;
        await _commonGroupQueries.UpdateAsync(group);
        return item;
    }

    public async new Task DeleteAsync(string id)
    {
        await _commonGroupQueries.DeleteAsync(id);
        var path = await base.DeleteAsync(id);
        Directory.Delete(path, true);
    }
}
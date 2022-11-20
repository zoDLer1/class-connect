using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public class TaskHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, FileEntity> _commonFileQueries;
    private CommonQueries<string, Work> _commonWorkQueries;
    
    public TaskHelperService(
        IHostEnvironment env, 
        ServiceResolver serviceAccessor,
        Context context) : base(env, serviceAccessor, context)
    {
        _commonFileQueries = new CommonQueries<string, FileEntity>(_context);
        _commonWorkQueries = new CommonQueries<string, Work>(_context);
    }

    public async Task<List<string>> HasAccessAsync(string id, User user, List<string> path)
    {
        await HasUserAccessToParentAsync(id, user, path);
        path.Add(id);
        return path;
    }

    private Object? GetWorkData(string parentId, User user)
    {
        var work = _context.Connections
            .Where(c => c.ParentId == parentId)
            .ToList()
            .Select(async c => await _commonWorkQueries.GetAsync(c.ChildId, _context.Works))
            .Select(c => c.Result)
            .FirstOrDefault(c => c != null && c.StudentId == user.Id);
        if (work == null)
            return null;
        var items = _context.WorkItems
            .Where(w => w.WorkId == work.Id)
            .ToList()
            .Select(async w => 
            {
                var item = await _common.GetAsync(w.ItemId, _context.Items.Include(e => e.Type));
                return new 
                {
                    Id = w.ItemId,
                    Name = item?.Name,
                    Type = item?.Type
                };
            })
            .ToList();
        return items;
    }

    public async Task<Object> GetAsync(string id, User user)
    {
        var folder = await base.GetFolderAsync(id, user);
        return new
        {
            Name = folder.Name,
            Type = folder.Type,
            Guid = folder.Guid,
            Path = folder.Path,
            Children = folder.Children,
            CreationTime = folder.CreationTime,
            Work = GetWorkData(id, user)
        };
    }

    public async virtual Task<Object> GetChildItemAsync(string id, User user)
    {
        var path = await HasAccessAsync(id, user, new List<string>());
        return await base.GetFolderInfoAsync(id);
    }

    public async Task<(string, Object)> CreateAsync(string parentId, string name, User user, Dictionary<string, string>? parameters=null)
    {
        await HasUserAccessToParentAsync(parentId, user, new List<string>());
        if (parentId == _rootGuid)
            throw new InvalidPathException();

        var parent = await TryGetItemAsync(parentId);
        if (parent.Type.Name != "Subject" && parent.Type.Name != "Folder")
            throw new InvalidPathException();
        
        var (itemPath, item) = await base.CreateAsync(parentId, name, 5, user);
        return (itemPath, await GetChildItemAsync(item.Guid, user));
    }

    public async new Task DeleteAsync(string id, User user)
    {
        var path = await base.DeleteAsync(id, user);
        Directory.Delete(path, true);
    }
}
using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public class WorkHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, Subject> _commonSubjectQueries;
    private CommonQueries<string, Work> _commonWorkQueries;
    
    public WorkHelperService(
        IHostEnvironment env, 
        ServiceResolver serviceAccessor,
        Context context) : base(env, serviceAccessor, context)
    {
        _commonWorkQueries = new CommonQueries<string, Work>(_context);
        _commonSubjectQueries = new CommonQueries<string, Subject>(_context);
    }

    public async Task<List<string>> HasAccessAsync(string id, User user, List<string> path)
    {
        await HasUserAccessToParentAsync(id, user, path);
        var item = await TryGetItemAsync(id);
        var work = await _commonWorkQueries.GetAsync(id, _context.Works.Include(w => w.Subject));
        if (work == null)
            throw new ItemNotFoundException();
        
        if (work.IsSubmitted && work.Subject.TeacherId == user.Id)
        {
            path.Add(work.Id);
            return path;
        }
        throw new AccessDeniedException();
    }

    public async Task<Object> GetAsync(string id, User user)
    {
        var path = await HasAccessAsync(id, user, new List<string>());
        var work = await _commonWorkQueries.GetAsync(id, _context.Works.Include(w => w.Subject));
        var folder = await base.GetFolderAsync(id, user);
        return new 
        {
            Name = folder.Name,
            Type = folder.Type,
            Path = folder.Path,
            Guid = folder.Guid,
            Children = folder.Children,
            CreationTime = folder.CreationTime,
            Mark = work?.Subject,
        };
    }

    public async virtual Task<Object> GetChildItemAsync(string id, User user)
    {
        await HasAccessAsync(id, user, new List<string>());
        return await base.GetFolderInfoAsync(id);
    }

    public async Task<(string, Object)> CreateAsync(string parentId, string name, User user, Dictionary<string, string>? parameters=null)
    {
        await HasUserAccessToParentAsync(parentId, user, new List<string>());
        var parent = await TryGetItemAsync(parentId);
        if (parent.Type.Name != "Task")
            throw new InvalidPathException();

        var ids = await GeneratePathAsync(parentId);
        string? subjectId = null;
        foreach (var id in ids)
            if (await _commonSubjectQueries.GetAsync(id, _context.Subjects) != null)
            {
                subjectId = id;
                break;
            }

        if (subjectId == null)
            throw new FolderNotFoundException();

        var (itemPath, item) = await base.CreateAsync(parentId, name, 6, user);
        var work = new Work
        {
            Id = item.Guid,
            SubjectId = subjectId,
            IsSubmitted = false
        };
        await _commonWorkQueries.CreateAsync(work);
        return (itemPath, await GetChildItemAsync(item.Guid, user));
    }

    public async new Task DeleteAsync(string id, User user)
    {
        var path = await base.DeleteAsync(id, user);
        await _commonWorkQueries.DeleteAsync(id);
        Directory.Delete(path, true);
    }
}
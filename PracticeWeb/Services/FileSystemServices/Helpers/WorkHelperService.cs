using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public class WorkHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, Subject> _commonSubjectQueries;
    private CommonQueries<string, Work> _commonWorkQueries;
    
    public WorkHelperService(IHostEnvironment env, Context context) : base(env, context)
    {
        _commonWorkQueries = new CommonQueries<string, Work>(_context);
        _commonSubjectQueries = new CommonQueries<string, Subject>(_context);
    }

    public async Task<(string, FolderItem)> CreateAsync(string parentId, string name, Dictionary<string, string>? parameters=null)
    {
        var parent = await TryGetItemAsync(parentId);
        if (parent.Type.Name != "Work")
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
        var (path, item) = await base.CreateAsync(parentId, name, 6);
        var work = new Work
        {
            Id = item.Guid,
            SubjectId = subjectId,
            IsSubmitted = false
        };
        await _commonWorkQueries.CreateAsync(work);
        return (path, item);
    }

    public async new Task DeleteAsync(string id)
    {
        await _commonWorkQueries.DeleteAsync(id);
        var path = await base.DeleteAsync(id);
        Directory.Delete(path, true);
    }
}
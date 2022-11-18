using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public class FolderHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, FileEntity> _commonFileQueries;
    
    public FolderHelperService(
        IHostEnvironment env, 
        ServiceResolver serviceAccessor,
        Context context) : base(env, serviceAccessor, context)
    {
        _commonFileQueries = new CommonQueries<string, FileEntity>(_context);
    }

    public async Task<List<string>> HasAccessAsync(string id, User user, List<string> path)
    {
        var item = await TryGetItemAsync(id);

        // Проверяем, если запрос от студента и он обращается ли к корню
        if (item.Id == _rootGuid)
        {
            if (user.Role.Name != "Student")
                path.Add(item.Id);
            return path;
        }

        await HasUserAccessToParentAsync(id, user, path);
        path.Add(item.Id);
        return path;
    }

    public async Task<Object> GetAsync(string id, User user)
    {
        var item = await TryGetItemAsync(id);

        // Проверяем, если запрос от студента и он обращается ли к корню
        var path = await HasAccessAsync(id, user, new List<string>());
        if (path.Count() == 0)
            throw new AccessDeniedException();

        var folder = await base.GetFolderAsync(id, user);
        folder.Path = await MakePathAsync(path);
        return folder;
    }

    public async Task<Object> GetChildItemAsync(string id, User user)
    {
        var item = await TryGetItemAsync(id);

        // Проверяем, если запрос от студента и он обращается ли к корню
        var path = await HasAccessAsync(id, user, new List<string>());
        if (user.Role.Name == "Student" && (path.Count() == 0 || id == _rootGuid))
            throw new AccessDeniedException();
            
        return await base.GetFolderInfoAsync(id);
    }

    public async Task<(string, Object)> CreateAsync(string parentId, string name, User user, Dictionary<string, string>? parameters=null)
    {
        await HasUserAccessToParentAsync(parentId, user, new List<string>());
        if (parentId == _rootGuid)
            throw new InvalidPathException();
        
        var parent = await TryGetItemAsync(parentId);
        if (parent.Type.Name != "Subject" && parent.Type.Name != "Folder" && parent.Type.Name != "Task")
            throw new InvalidPathException();
        
        var (itemPath, item) = await base.CreateAsync(parentId, name, 1, user);
        return (itemPath, await GetChildItemAsync(item.Guid, user));
    }

    public async new Task DeleteAsync(string id, User user)
    {
        var path = await base.DeleteAsync(id, user);
        Directory.Delete(path, true);
    }
}
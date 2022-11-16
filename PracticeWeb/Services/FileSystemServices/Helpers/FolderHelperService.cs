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

    public async virtual Task<Object> GetChildItemAsync(string id, User user)
    {
        var item = await TryGetItemAsync(id);

        // Проверяем, если запрос от студента и он обращается ли к корню
        Console.WriteLine("aaaaaaaa");
        var path = await HasAccessAsync(id, user, new List<string>());
        if (user.Role.Name == "Student" && path.Count() == 0)
            throw new AccessDeniedException();
        Console.WriteLine("bbbbbbbb");
            
        return await base.GetFolderInfoAsync(id);
    }

    public async Task<(string, FolderItem)> CreateAsync(string parentId, string name, User user, Dictionary<string, string>? parameters=null)
    {
        var result = await base.CreateAsync(parentId, name, 1, user.Id);
        return result;
    }

    public async new Task DeleteAsync(string id)
    {
        var path = await base.DeleteAsync(id);
        Directory.Delete(path, true);
    }
}
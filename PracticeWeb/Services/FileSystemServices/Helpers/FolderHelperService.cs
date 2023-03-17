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

    public async Task<ItemAccess> HasAccessAsync(string id, User user, List<string> path)
    {
        var item = await TryGetItemAsync(id);

        // Проверяем, если запрос от админа или преподавателя и обращается ли он к корню
        if (item.Id == _rootGuid)
        {
            var permission = await GetPermission(user.Id, item.Id);
            if (permission != Permission.None || user.Role.Id == UserRole.Teacher)
            {
                path.Add(item.Id);
                if (permission == Permission.None)
                    permission = Permission.Read;
            }

            return new ItemAccess { Permission = permission, Path = path };
        }

        var parentAccess = await HasUserAccessToParentAsync(id, user, path);
        path.Add(item.Id);
        return parentAccess;
    }

    public async Task<Object> GetAsync(string id, User user)
    {
        var access = await HasAccessAsync(id, user, new List<string>());
        // Проверяем, если запрос от студента и он обращается ли к корню
        if (user.Role.Id != UserRole.Teacher && access.Permission == Permission.None || access.Path.Count() == 0)
            throw new AccessDeniedException();

        var folder = await base.GetFolderAsync(id, user);
        folder.Path = await MakePathAsync(access.Path);
        return folder;
    }

    public async Task<Object> GetChildItemAsync(string id, User user)
    {
        // Проверяем, если запрос от студента и он обращается ли к корню
        var access = await HasAccessAsync(id, user, new List<string>());
        if (user.Role.Id != UserRole.Teacher && access.Permission == Permission.None || access.Path.Count() == 0)
            throw new AccessDeniedException();

        return await base.GetFolderInfoAsync(id);
    }

    public async Task<(string, Object)> CreateAsync(string parentId, string name, User user, Dictionary<string, string>? parameters=null)
    {
        // Если пытаемся создать папку в руте
        if (parentId == _rootGuid)
            throw new InvalidPathException();

        var parent = await TryGetItemAsync(parentId);
        var access = await _serviceAccessor(parent.Type.Name).HasAccessAsync(parent.Id, user, new List<string>());

        if (access.Permission != Permission.Write)
            throw new AccessDeniedException();

        // Проверка допустимости типов
        if (!TypeDependence.Folder.Contains(parent.TypeId))
            throw new InvalidPathException();

        var (itemPath, item) = await base.CreateAsync(parentId, name, Type.Folder, user);
        return (itemPath, await GetChildItemAsync(item.Guid, user));
    }

    public async new Task DeleteAsync(string id, User user)
    {
        var path = await base.DeleteAsync(id, user);
        Directory.Delete(path, true);
    }
}

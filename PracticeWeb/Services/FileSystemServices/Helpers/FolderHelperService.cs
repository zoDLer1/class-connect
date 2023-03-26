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

            Console.WriteLine($"folder access: {permission} in {id}");
            return new ItemAccess { Permission = permission, Path = path };
        }

        var access = await HasUserAccessToParentAsync(id, user, path);
        Console.WriteLine($"folder access: {access.Permission} in {id}");
        path.Add(item.Id);
        return access;
    }

    public async Task<object> GetAsync(string id, User user, Boolean asChild)
    {
        var access = await HasAccessAsync(id, user, new List<string>());
        // Проверяем, если запрос от студента и он обращается ли к корню
        if (user.Role.Id != UserRole.Teacher && access.Permission == Permission.None || access.Path.Count() == 0)
            throw new AccessDeniedException();

        var folder = await base.GetFolderAsync(id, user, asChild);
        // В корне можно создать только группы
        if (user.RoleId == UserRole.Administrator && access.Path.Count() == 1)
            folder.Access = new List<string> { "Group" };
        else
            folder.Access.Remove("Group");

        return folder;
    }

    public async Task CheckIfCanCreateAsync(string parentId, User user)
    {
        if (parentId == _rootGuid)
            throw new InvalidPathException();

        await base.CheckIfCanCreateAsync(parentId, Type.Folder, user);
    }

    public async Task<(string, object)> CreateAsync(string parentId, string name, User user, Dictionary<string, object>? parameters=null)
    {
        await CheckIfCanCreateAsync(parentId, user);
        var (itemPath, item) = await base.CreateAsync(parentId, name, Type.Folder, user);
        var parent = await TryGetItemAsync(parentId);
        return (itemPath, await _serviceAccessor(parent.TypeId).GetAsync(parentId, user, false));
    }

    public async new Task DeleteAsync(string id, User user)
    {
        var path = await base.DeleteAsync(id, user);
        Directory.Delete(path, true);
    }
}

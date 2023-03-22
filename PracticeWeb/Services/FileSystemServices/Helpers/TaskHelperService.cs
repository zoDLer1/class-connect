using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public class TaskHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, TaskEntity> _commonTaskQueries;
    private CommonQueries<string, FileEntity> _commonFileQueries;
    private CommonQueries<string, Work> _commonWorkQueries;

    public TaskHelperService(
        IHostEnvironment env,
        ServiceResolver serviceAccessor,
        Context context) : base(env, serviceAccessor, context)
    {
        _commonTaskQueries = new CommonQueries<string, TaskEntity>(_context);
        _commonFileQueries = new CommonQueries<string, FileEntity>(_context);
        _commonWorkQueries = new CommonQueries<string, Work>(_context);
    }

    public async Task<ItemAccess> HasAccessAsync(string id, User user, List<string> path)
    {
        var access = await HasUserAccessToParentAsync(id, user, path);
        Console.WriteLine($"task access: {access.Permission} in {id}");
        access.Path.Add(id);
        return access;
    }

    public async Task<object> GetAsync(string id, User user)
    {
        var access = await HasAccessAsync(id, user, new List<string>());
        var task = await _commonTaskQueries.GetAsync(id, _context.Tasks);
        var folder = await base.GetFolderAsync(id, user);

        var workConnection = await _context.Connections
            .Include(c => c.Child)
            .FirstOrDefaultAsync(c => c.ParentId == id && c.Child.CreatorId == user.Id);

        return new
        {
            Name = folder.Name,
            Type = folder.Type,
            Guid = folder.Guid,
            Path = folder.Path,
            Children = folder.Children,
            CreationTime = folder.CreationTime,
            CreatorName = folder.CreatorName,
            Until = task?.Until,
            Work = workConnection != null ? await GetWorkData(workConnection.ChildId, user) : null
        };
    }

    public async virtual Task<object> GetChildItemAsync(string id, User user)
    {
        var access = await HasAccessAsync(id, user, new List<string>());
        var task = await _commonTaskQueries.GetAsync(id, _context.Tasks);
        var folderItem = await base.GetFolderInfoAsync(id);
        return new
        {
            Name = folderItem.Name,
            Type = folderItem.Type,
            Guid = folderItem.Guid,
            CreationTime = folderItem.CreationTime,
            CreatorName = folderItem.CreatorName,
            Until = task?.Until
        };
    }

    public async Task<(string, object)> CreateAsync(string parentId, string name, User user, Dictionary<string, object>? parameters=null)
    {
        // Если пытаемся создать файл в руте
        if (parentId == _rootGuid)
            throw new InvalidPathException();

        var parent = await TryGetItemAsync(parentId);
        var access = await _serviceAccessor(parent.Type.Id).HasAccessAsync(parent.Id, user, new List<string>());

        if (access.Permission != Permission.Write)
            throw new AccessDeniedException();

        // Проверка допустимости типов
        if (!TypeDependence.Task.Contains(parent.TypeId))
            throw new InvalidPathException();

        DateTime? until = null;
        if (parameters?.ContainsKey("Until") == true)
        {
            until = parameters["Until"] as DateTime?;

            if (until == null)
                throw new InvalidDataException();

            // Учитываем текущий часовой пояс
            until = ((DateTime) until).AddHours(3);
            if (until <= DateTime.Now.AddHours(2))
                throw new InvalidDataException();
        }

        var (itemPath, item) = await base.CreateAsync(parentId, name, Type.Task, user);
        var task = new TaskEntity
        {
            Id = item.Guid,
            Until = until
        };
        await _commonTaskQueries.CreateAsync(task);
        return (itemPath, await GetChildItemAsync(item.Guid, user));
    }

    public async new Task DeleteAsync(string id, User user)
    {
        var path = await base.DeleteAsync(id, user);
        await _commonTaskQueries.DeleteAsync(id);
        Directory.Delete(path, true);
    }
}

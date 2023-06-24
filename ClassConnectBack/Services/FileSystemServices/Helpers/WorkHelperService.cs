using Microsoft.EntityFrameworkCore;
using ClassConnect.Exceptions;
using ClassConnect.Models;

namespace ClassConnect.Services.FileSystemServices.Helpers;

public class WorkHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, FileEntity> _commonFileQueries;
    private CommonQueries<string, Subject> _commonSubjectQueries;
    private CommonQueries<string, Work> _commonWorkQueries;

    public WorkHelperService(IHostEnvironment env, ServiceResolver serviceAccessor, Context context)
        : base(env, serviceAccessor, context)
    {
        _commonFileQueries = new CommonQueries<string, FileEntity>(_context);
        _commonWorkQueries = new CommonQueries<string, Work>(_context);
        _commonSubjectQueries = new CommonQueries<string, Subject>(_context);
    }

    public async Task<ItemAccess> HasAccessAsync(string id, User user, List<string> path)
    {
        var access = await HasUserAccessToParentAsync(id, user, path);
        var work = await _commonWorkQueries.GetAsync(id, _context.Works);

        if (work == null)
            throw new ItemNotFoundException();

        var item = await TryGetItemAsync(id);
        // Если пользователь не является создателем работы
        if (item.CreatorId != user.Id)
            // То преподаватели и администраторы могут видеть работу, если она сдана
            if (work.IsSubmitted && user.RoleId != UserRole.Student)
                if (user.RoleId == UserRole.Administrator)
                    access.Permission = Permission.Write;
                else
                    access.Permission = Permission.Read;
            // Иначе другие пользователи не имеют права на просмотр
            else
                throw new AccessDeniedException();
        else if (!work.IsSubmitted)
            access.Permission = Permission.Write;

        Console.WriteLine($"work access: {access.Permission} in {id}");
        access.Path.Add(work.Id);
        return access;
    }

    public async Task<object> GetAsync(string id, User user, Boolean asChild)
    {
        var access = await HasAccessAsync(id, user, new List<string>());
        var work = await _commonWorkQueries.GetAsync(id, _context.Works);

        // Если доступ запрашивает не студент и работа сдана, то показываем
        if (!(work?.IsSubmitted == true && user.RoleId != UserRole.Student))
            throw new AccessDeniedException();

        var parentConnection = await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == id);
        if (parentConnection == null)
            throw new ItemNotFoundException();

        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == parentConnection.ParentId);
        if (task == null)
            throw new ItemNotFoundException();

        var folder = await base.GetFolderAsync(id, user, asChild);
        return new
        {
            Name = folder.Name,
            Type = folder.Type,
            Guid = folder.Guid,
            Path = folder.Path,
            Children = folder.Children,
            Data = new
            {
                CreationTime = folder.Data.CreationTime,
                CreatorName = folder.Data.CreatorName,
                Mark = work?.Mark,
                SubmitTime = work?.SubmitDate,
                IsLate = work?.SubmitDate > task.Until,
            },
            Access = folder.Access,
            IsEditable = false
        };
    }

    public async Task CheckIfCanCreateAsync(string parentId, User user)
    {
        if (user.RoleId != UserRole.Student)
            throw new AccessDeniedException();

        if (parentId == _rootGuid)
            throw new InvalidPathException();

        var parent = await TryGetItemAsync(parentId);
        var access = await _serviceAccessor(parent.Type.Id)
            .HasAccessAsync(parent.Id, user, new List<string>());

        // Проверка допустимости типов
        if (!TypeDependence.Work.Contains(parent.TypeId))
            throw new InvalidPathException();

        var oldWork = await _context.Connections
            .Include(c => c.Child)
            .FirstOrDefaultAsync(c => c.ParentId == parentId && c.Child.CreatorId == user.Id);
        if (oldWork != null)
        {
            var work = await _commonWorkQueries.GetAsync(oldWork.ChildId, _context.Works);
            if (work?.IsSubmitted == true)
                throw new AccessDeniedException();
        }
    }

    public async Task<(string, object)> CreateAsync(
        string parentId,
        string name,
        User user,
        Dictionary<string, object>? parameters = null
    )
    {
        await CheckIfCanCreateAsync(parentId, user);

        var oldWork = await _context.Connections
            .Include(c => c.Child)
            .FirstOrDefaultAsync(c => c.ParentId == parentId && c.Child.CreatorId == user.Id);
        if (oldWork != null)
            throw new FolderNotFoundException();

        var (itemPath, item) = await base.CreateAsync(parentId, name, Type.Work, user);
        var work = new Work { Id = item.Guid };
        await _commonWorkQueries.CreateAsync(work);
        var data = await GetWorkData(item.Guid, user);
        if (data == null)
            throw new FolderNotFoundException();
        return (itemPath, data);
    }

    public override Task<FolderItem> UpdateAsync(string id, string newName, User user)
    {
        throw new AccessDeniedException();
    }

    public new Task DeleteAsync(string id, User user)
    {
        throw new AccessDeniedException();
    }
}

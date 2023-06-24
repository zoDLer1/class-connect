using Microsoft.EntityFrameworkCore;
using ClassConnect.Exceptions;
using ClassConnect.Models;

namespace ClassConnect.Services.FileSystemServices.Helpers;

public class GroupHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, Group> _commonGroupQueries;

    public GroupHelperService(
        IHostEnvironment env,
        ServiceResolver serviceAccessor,
        Context context
    )
        : base(env, serviceAccessor, context)
    {
        _commonGroupQueries = new CommonQueries<string, Group>(_context);
    }

    public async Task<ItemAccess> HasAccessAsync(string id, User user, List<string> path)
    {
        var group = await _commonGroupQueries.GetAsync(id, _context.Groups);
        if (group == null)
            throw new ItemNotFoundException();

        var access = await HasUserAccessToParentAsync(id, user, path);
        var permission = await GetPermission(user.Id, group.Id);

        // Обнуляем доступ, поскольку родитель является корнем и весь трафик идёт через группу
        if (access.Permission < Permission.Write)
            access.Permission = Permission.None;

        // Установлен ли доступ пользователю?
        if (permission != Permission.None)
            access.Permission = permission;

        // Ведёт ли преподаватель предмет?
        if (
            Permission.Write > access.Permission
            && await _context.Subjects.FirstOrDefaultAsync(
                s => s.TeacherId == user.Id && s.GroupId == group.Id
            ) != null
        )
            access.Permission = Permission.Read;

        Console.WriteLine($"group access: {access.Permission} or {permission} in {id}");
        if (access.Permission == Permission.None)
            throw new AccessDeniedException();

        access.Path.Add(group.Id);
        return access;
    }

    private object GetGroupData(Group group, User user, Boolean asChild)
    {
        var subjects = _context.Subjects.Include(s => s.Item).Where(s => s.GroupId == group.Id);
        if (user.RoleId != UserRole.Administrator && group.TeacherId != user.Id)
            subjects = subjects.Where(s => s.TeacherId == user.Id);

        return new
        {
            CreationTime = group.Item.CreationTime,
            Teacher = new
            {
                Id = group.Teacher.Id,
                Name = group.Teacher.Name,
                Surname = group.Teacher.Surname,
                Patronymic = group.Teacher.Patronymic
            },
            Subjects = asChild
                ? subjects
                    .Select(s => new { Id = s.Id, Name = s.Item.Name, })
                    .OrderBy(s => s.Name)
                    .ToList<object>()
                : new List<object>(),
            Students = new
            {
                IsEditable = user.Id == group.TeacherId || user.RoleId == UserRole.Administrator,
                Items = _context.Accesses
                    .Where(s => s.ItemId == group.Id && s.Permission == Permission.Read)
                    .ToList()
                    .Select(s =>
                    {
                        var user = _context.Users.FirstOrDefault(u => u.Id == s.UserId);
                        return new
                        {
                            Id = user?.Id,
                            Name = string.Join(
                                ' ',
                                new[] { user?.Name, user?.Surname, user?.Patronymic }
                            )
                        };
                    })
                    .OrderBy(s => s.Name)
            }
        };
    }

    public async Task<object> GetAsync(string id, User user, Boolean asChild)
    {
        var access = await HasAccessAsync(id, user, new List<string>());
        var item = await TryGetItemAsync(id);
        var group = await _commonGroupQueries.GetAsync(id, _context.Groups.Include(g => g.Teacher));
        var folder = await base.GetFolderAsync(id, user, asChild);
        return new
        {
            Name = folder.Name,
            Type = folder.Type,
            Guid = folder.Guid,
            Path = folder.Path,
            Children = asChild ? null : folder.Children,
            Data = user.Role.Id == UserRole.Student || group == null
                ? null
                : GetGroupData(group, user, asChild),
            Access = folder.Access,
            IsEditable = folder.IsEditable
        };
    }

    public async Task CheckIfCanCreateAsync(string parentId, User user)
    {
        if (user.RoleId != UserRole.Administrator)
            throw new AccessDeniedException();

        await base.CheckIfCanCreateAsync(parentId, Type.Group, user);

        // Является ли родитель корнем
        if (
            await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == parentId) != null
            && parentId != _rootGuid
        )
            throw new InvalidPathException();
    }

    public async Task<(string, object)> CreateAsync(
        string parentId,
        string name,
        User user,
        Dictionary<string, object>? parameters = null
    )
    {
        await CheckIfCanCreateAsync(parentId, user);

        // Еесть ли группа с таким же названием
        if (
            await _context.Groups.Include(g => g.Item).FirstOrDefaultAsync(g => g.Item.Name == name)
            != null
        )
            throw new InvalidGroupNameException();

        if (parameters?.ContainsKey("TeacherId") == false)
            throw new NullReferenceException();

        int? teacherId = parameters?["TeacherId"] as int?;
        var teacher = _context.Users.Include(s => s.Role).FirstOrDefault(s => s.Id == teacherId);
        if (teacher == null)
            throw new TeacherNotFoundException();

        if (teacher.Role.Id != UserRole.Teacher)
            throw new InvalidUserRoleException();

        var (itemPath, item) = await base.CreateAsync(parentId, name, Type.Group, user);
        var group = new Group { Id = item.Guid, TeacherId = teacher.Id, };
        await _commonGroupQueries.CreateAsync(group);
        var teacherAccess = new Access
        {
            Permission = Permission.Write,
            ItemId = group.Id,
            UserId = teacher.Id,
        };
        _context.Accesses.Add(teacherAccess);
        await _context.SaveChangesAsync();
        var parent = await TryGetItemAsync(parentId);
        return (itemPath, await _serviceAccessor(parent.TypeId).GetAsync(parentId, user, false));
    }

    public async override Task<FolderItem> UpdateAsync(string id, string newName, User user)
    {
        var access = await HasAccessAsync(id, user, new List<string>());
        if (access.Permission != Permission.Write)
            throw new AccessDeniedException();

        var group = await _commonGroupQueries.GetAsync(id, _context.Groups);
        if (group == null)
            throw new ItemNotFoundException();

        // Есть ли группа с таким же названием
        if (
            await _context.Groups
                .Include(g => g.Item)
                .FirstOrDefaultAsync(g => g.Item.Name == newName) != null
        )
            throw new InvalidGroupNameException();

        var item = await base.UpdateAsync(id, newName, user);
        await _commonGroupQueries.UpdateAsync(group);
        return item;
    }

    public async new Task DeleteAsync(string id, User user)
    {
        var path = await base.DeleteAsync(id, user);
        Directory.Delete(path, true);
    }
}

using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public class GroupHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, Group> _commonGroupQueries;

    public GroupHelperService(
        IHostEnvironment env,
        ServiceResolver serviceAccessor,
        Context context) : base(env, serviceAccessor, context)
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
        if (Permission.Write > access.Permission && await _context.Subjects.FirstOrDefaultAsync(s => s.TeacherId == user.Id && s.GroupId == group.Id) != null)
            access.Permission = Permission.Read;

        Console.WriteLine($"group access: {access.Permission} or {permission} in {id}");

        if (access.Permission == Permission.None)
            throw new AccessDeniedException();

        access.Path.Add(group.Id);
        return access;
    }

    private object GetGroupData(Group group, User user)
    {
        var subjects = _context.Subjects.Where(s => s.GroupId == group.Id);
        if (group.TeacherId != user.Id)
            subjects = subjects.Where(s => s.TeacherId == user.Id);
        return new
        {
            Subjects = subjects
            .ToList()
            .Select(async s => new
            {
                Id = s.Id,
                Name = (await TryGetItemAsync(s.Id)).Name,
                Students = _context.Accesses
                    .Where(s => s.ItemId == group.Id && s.Permission == Permission.Read)
                    .ToList()
                    .Select(s => {
                        var user = _context.Users.FirstOrDefault(u => u.Id == s.UserId);
                        return new {
                            Id = user?.Id,
                            Name = string.Join(' ', new[] { user?.Name, user?.Surname, user?.Patronymic })
                        };
                    })
            })
            .Select(s => s.Result)
        };
    }

    public async Task<object> GetAsync(string id, User user)
    {
        var access = await HasAccessAsync(id, user, new List<string>());
        var group = await _commonGroupQueries.GetAsync(id, _context.Groups.Include(g => g.Teacher));
        var folder = await base.GetFolderAsync(id, user);
        return new
        {
            Name = folder.Name,
            Type = folder.Type,
            Guid = folder.Guid,
            Path = folder.Path,
            Children = folder.Children,
            CreationTime = folder.CreationTime,
            Teacher = new {
                Id = group?.Teacher.Id,
                FirstName = group?.Teacher.Name,
                LastName = group?.Teacher.Surname,
                Patronymic = group?.Teacher.Patronymic
            },
            Data = user.Role.Id == UserRole.Student || group == null ? null : GetGroupData(group, user),
            Access = folder.Access
        };
    }

    public async Task<object> GetChildItemAsync(string id, User user)
    {
        var access = await HasAccessAsync(id, user, new List<string>());
        var group = await _commonGroupQueries.GetAsync(id, _context.Groups);
        var folderItem = await base.GetFolderInfoAsync(id);
        return new
        {
            Name = folderItem.Name,
            Type = folderItem.Type,
            Guid = folderItem.Guid,
            CreationTime = folderItem.CreationTime,
            Teacher = group?.TeacherId,
            Data = user.Role.Id == UserRole.Student || group == null ? null : GetGroupData(group, user)
        };
    }

    public async Task CheckIfCanCreateAsync(string parentId, User user)
    {
        if (user.RoleId != UserRole.Administrator)
            throw new AccessDeniedException();

        await base.CheckIfCanCreateAsync(parentId, Type.Group, user);

        // Является ли родитель корнем
        if (await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == parentId) != null && parentId != _rootGuid)
            throw new InvalidPathException();
    }

    public async Task<(string, object)> CreateAsync(string parentId, string name, User user, Dictionary<string, object>? parameters=null)
    {
        await CheckIfCanCreateAsync(parentId, user);

        // Еесть ли группа с таким же названием
        if (await _context.Groups.Include(g => g.Item).FirstOrDefaultAsync(g => g.Item.Name == name) != null)
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
        var group = new Group
        {
            Id = item.Guid,
            TeacherId = teacher.Id,
        };
        await _commonGroupQueries.CreateAsync(group);
        var teacherAccess = new Access {
            Permission = Permission.Write,
            ItemId = group.Id,
            UserId = teacher.Id,
        };
        _context.Accesses.Add(teacherAccess);
        await _context.SaveChangesAsync();
        return (itemPath, await GetChildItemAsync(item.Guid, user));
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
        if (await _context.Groups.Include(g => g.Item).FirstOrDefaultAsync(g => g.Item.Name == newName) != null)
            throw new InvalidGroupNameException();

        var item = await base.UpdateAsync(id, newName, user);
        await _commonGroupQueries.UpdateAsync(group);
        return item;
    }

    public async new Task DeleteAsync(string id, User user)
    {
        var path = await base.DeleteAsync(id, user);
        await _commonGroupQueries.DeleteAsync(id);
        Directory.Delete(path, true);
    }
}

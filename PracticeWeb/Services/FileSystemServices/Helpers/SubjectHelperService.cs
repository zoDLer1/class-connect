using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public class SubjectHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, Group> _commonGroupQueries;
    private CommonQueries<string, Subject> _commonSubjectQueries;

    public SubjectHelperService(
        IHostEnvironment env,
        ServiceResolver serviceAccessor,
        Context context) : base(env, serviceAccessor, context)
    {
        _commonGroupQueries = new CommonQueries<string, Group>(_context);
        _commonSubjectQueries = new CommonQueries<string, Subject>(_context);
    }

    public async Task<ItemAccess> HasAccessAsync(string id, User user, List<string> path)
    {
        var subject = await _commonSubjectQueries.GetAsync(id, _context.Subjects.Include(s => s.Group));
        if (subject == null)
            throw new ItemNotFoundException();

        var access = await HasUserAccessToParentAsync(id, user, path);
        var permission = await GetPermission(user.Id, subject.Id);

        // Если это преподаватель и он не имеет доступ к группе на запись и не имеет доступ на запись к предмету
        if (user.RoleId == UserRole.Teacher && Permission.Write > access.Permission && permission == Permission.None)
            access.Permission = Permission.None;

        Console.WriteLine($"subject access: {access.Permission} or {permission} in {id}");

        if (access.Permission == Permission.None)
            throw new AccessDeniedException();

        // Установлен ли доступ пользователю?
        if (permission != Permission.None)
            access.Permission = permission;

        access.Path.Add(subject.Id);
        return access;
    }

    public async Task<Object> GetAsync(string id, User user)
    {
        var access = await HasAccessAsync(id, user, new List<string>());
        var subject = await _commonSubjectQueries.GetAsync(id, _context.Subjects.Include(s => s.Group));
        var folder = await base.GetFolderAsync(id, user);
        return new
        {
            Name = folder.Name,
            Type = folder.Type,
            Guid = folder.Guid,
            Path = folder.Path,
            Children = folder.Children,
            CreationTime = folder.CreationTime,
            Group = subject?.Group.Name,
            Teacher = subject?.TeacherId,
            Description = subject?.Description
        };
    }

    public async virtual Task<Object> GetChildItemAsync(string id, User user)
    {
        var access = await HasAccessAsync(id, user, new List<string>());
        var subject = await _commonSubjectQueries.GetAsync(id, _context.Subjects.Include(s => s.Group).Include(s => s.Teacher));
        var folderItem = await base.GetFolderInfoAsync(id);
        return new
        {
            Name = folderItem.Name,
            Type = folderItem.Type,
            Guid = folderItem.Guid,
            CreationTime = folderItem.CreationTime,
            Group = subject?.Group.Name,
            Teacher = new {
                Id = subject?.Teacher.Id,
                FirstName = subject?.Teacher.FirstName,
                LastName = subject?.Teacher.LastName,
                Patronymic = subject?.Teacher.Patronymic
            },
            Description = subject?.Description
        };
    }

    public async Task<(string, Object)> CreateAsync(string parentId, string name, User user, Dictionary<string, string>? parameters=null)
    {
        var access = await HasUserAccessToParentAsync(parentId, user, new List<string>());
        if (access == null || access.Permission != Permission.Write)
            throw new AccessDeniedException();

        var parent = await TryGetItemAsync(parentId);
        // Проверка допустимости типов
        if (!TypeDependence.Subject.Contains(parent.TypeId))
            throw new InvalidPathException();

        // Проверяем, является ли родитель папкой
        var group = await _commonGroupQueries.GetAsync(parentId, _context.Groups);
        if (group == null)
            throw new InvalidPathException();

        // Проверяем, есть ли у данной группы такой предмет
        var groupSubjects = await _context.Subjects.Where(s => s.GroupId == parentId).ToListAsync();
        var anotherSubject = groupSubjects.FirstOrDefault(g => g.Name == name);
        if (anotherSubject != null)
            throw new InvalidSubjectNameException();

        if (parameters?.ContainsKey("TeacherId") == false)
        throw new NullReferenceException();

        int teacherId = 0;
        if (!int.TryParse(parameters?["TeacherId"], out teacherId))
            throw new NullReferenceException();

        var teacher = _context.Users.Include(s => s.Role).FirstOrDefault(s => s.Id == teacherId);
        if (teacher == null)
            throw new UserNotFoundException();

        if (teacher.Role.Id != UserRole.Teacher)
            throw new InvalidUserRoleException();

        var (itemPath, item) = await base.CreateAsync(parentId, name, Type.Subject, user);
        var subject = new Subject
        {
            Id = item.Guid,
            GroupId = group.Id,
            Name = name,
            TeacherId = teacherId,
            Description = parameters?.ContainsKey("Description") == true ? parameters["Description"] : null
        };
        await _commonSubjectQueries.CreateAsync(subject);
        var teacherAccess = new Access {
            Permission = Permission.Write,
            ItemId = subject.Id,
            UserId = subject.TeacherId,
        };
        _context.Accesses.Add(teacherAccess);
        await _context.SaveChangesAsync();
        return (itemPath, await GetChildItemAsync(item.Guid, user));
    }

    public async override Task<FolderItem> UpdateAsync(string id, string newName, User user)
    {
        if (user.Role.Name != "Administrator")
            throw new AccessDeniedException();

        var subject = await _commonSubjectQueries.GetAsync(id, _context.Subjects);
        if (subject == null)
            throw new ItemNotFoundException();

        // Проверяем, есть ли у данной группы такой предмет
        var groupSubjects = await _context.Subjects.Where(s => s.GroupId == subject.GroupId).ToListAsync();
        var anotherSubject = groupSubjects.FirstOrDefault(g => g.Name == newName);
        if (anotherSubject != null)
            throw new InvalidSubjectNameException();

        var item = await base.UpdateAsync(id, newName, user);
        subject.Name = newName;
        await _commonSubjectQueries.UpdateAsync(subject);
        return item;
    }

    public async new Task DeleteAsync(string id, User user)
    {
        var path = await base.DeleteAsync(id, user);
        await _commonSubjectQueries.DeleteAsync(id);
        Directory.Delete(path, true);
    }
}

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

    public async Task<List<string>> HasAccessAsync(string id, User user, List<string> path)
    {
        var group = await _commonGroupQueries.GetAsync(id, _context.Groups);
        if (group == null)
            throw new ItemNotFoundException();

        Console.WriteLine($"group: {id} check if teacher in this group {group.TeacherId == user.Id} and he has a subject {await _context.Subjects.FirstOrDefaultAsync(s => s.TeacherId == user.Id && s.GroupId == group.Id) != null}");
        // Проверяем, является ли преподаватель руководителем группы или ведёт ли какой-нибудь предмет
        if (user.Role.Name == "Teacher" && !(group.TeacherId == user.Id || 
            await _context.Subjects.FirstOrDefaultAsync(s => s.TeacherId == user.Id && s.GroupId == group.Id) != null))
            throw new AccessDeniedException();
        // Проверяем, есть ли студент в данной группе
        if (user.Role.Name == "Student" && 
            await _context.GroupStudents.FirstOrDefaultAsync(s => s.StudentId == user.Id && s.GroupId == group.Id) == null)
            throw new AccessDeniedException();

        await HasUserAccessToParentAsync(id, user, path);
        path.Add(group.Id);
        return path;
    }

    private Object GetGroupData(string id)
    {
        return new 
        {
            Subjects = _context.Subjects.Where(s => s.GroupId == id).Select(s => new 
            {
                Id = s.Id,
                Name = s.Name
            }),
            Students = _context.GroupStudents
                .Where(s => s.GroupId == id)
                .ToList()
                .Select(s => {
                    var user = _context.Users.First(u => u.Id == s.StudentId);
                    return new { 
                        Id = user.Id, 
                        Name = string.Join(' ', new[] { user.FirstName, user.LastName, user.Patronymic })
                    };
                })
                .ToList()
        };
    }

    public async Task<Object> GetAsync(string id, User user)
    {
        var path = await HasAccessAsync(id, user, new List<string>());
        var group = await _commonGroupQueries.GetAsync(id, _context.Groups);
        var folder = await base.GetFolderAsync(id, user);
        return new 
        {
            Name = folder.Name,
            Type = folder.Type,
            Path = folder.Path,
            Guid = folder.Guid,
            Children = folder.Children,
            CreationTime = folder.CreationTime,
            Teacher = group?.TeacherId,
            Data = user.Role.Name == "Student" ? null : GetGroupData(id)
        };
    }

    public async virtual Task<Object> GetChildItemAsync(string id, User user)
    {
        var path = await HasAccessAsync(id, user, new List<string>());
        var group = await _commonGroupQueries.GetAsync(id, _context.Groups);
        var folderItem = await base.GetFolderInfoAsync(id);
        return new 
        {
            Name = folderItem.Name,
            Type = folderItem.Type,
            Guid = folderItem.Guid,
            CreationTime = folderItem.CreationTime,
            Teacher = group?.TeacherId,
            Data = user.Role.Name == "Student" ? null : GetGroupData(id)
        };
    }

    public async Task<(string, FolderItem)> CreateAsync(string parentId, string name, User user, Dictionary<string, string>? parameters=null)
    {
        // Проверяем, является ли родитель корнем
        if (await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == parentId) != null)
            throw new InvalidPathException();

        if (await _context.Groups.FirstOrDefaultAsync(g => g.Name == name) != null)
            throw new InvalidGroupNameException();

        if (parameters?.ContainsKey("TeacherId") == false)
            throw new NullReferenceException();
        
        int teacherId = 0;
        if (!int.TryParse(parameters?["TeacherId"], out teacherId))
            throw new NullReferenceException();
        
        var (path, item) = await base.CreateAsync(parentId, name, 3, user.Id);
        var group = new Group
        {
            Id = item.Guid,
            Name = name,
            TeacherId = teacherId,
        };
        await _commonGroupQueries.CreateAsync(group);
        return (path, item);
    }

    public async override Task<FolderItem> UpdateAsync(string id, string newName)
    {
        var group = await _commonGroupQueries.GetAsync(id, _context.Groups);
        if (group == null)
            throw new ItemNotFoundException();

        var anotherGroup = await _context.Groups.FirstOrDefaultAsync(s => s.Name == newName);
        if (anotherGroup != null)
            throw new InvalidGroupNameException();

        var item = await base.UpdateAsync(id, newName);
        group.Name = newName;
        await _commonGroupQueries.UpdateAsync(group);
        return item;
    }

    public async new Task DeleteAsync(string id)
    {
        await _commonGroupQueries.DeleteAsync(id);
        var path = await base.DeleteAsync(id);
        Directory.Delete(path, true);
    }
}
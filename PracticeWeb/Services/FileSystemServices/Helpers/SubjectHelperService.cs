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

    public async Task<List<string>> HasAccessAsync(string id, User user, List<string> path)
    {
        var subject = await _commonSubjectQueries.GetAsync(id, _context.Subjects.Include(s => s.Group));
        if (subject == null)
            throw new ItemNotFoundException();

        // Проверяем, является ли преподаватель руководителем группы или ведёт ли данный предмет
        if (user.Role.Name == "Teacher" && !(subject.Group.TeacherId == user.Id || subject.TeacherId == user.Id))
            throw new AccessDeniedException();

        await HasUserAccessToParentAsync(id, user, path);
        path.Add(subject.Id);
        return path;
    }

    public async Task<Object> GetAsync(string id, User user)
    {
        var path = await HasAccessAsync(id, user, new List<string>());
        var subject = await _commonSubjectQueries.GetAsync(id, _context.Subjects.Include(s => s.Group));
        var folder = await base.GetFolderAsync(id, user);
        return new 
        {
            Name = folder.Name,
            Type = folder.Type,
            Path = folder.Path,
            RealPath = path,
            Guid = folder.Guid,
            Children = folder.Children,
            CreationTime = folder.CreationTime,
            Group = subject?.Group.Name,
            Teacher = subject?.TeacherId,
            Description = subject?.Description
        };
    }

    public async virtual Task<Object> GetChildItemAsync(string id, User user)
    {
        var path = await HasAccessAsync(id, user, new List<string>());
        var subject = await _commonSubjectQueries.GetAsync(id, _context.Subjects.Include(s => s.Group));
        var folderItem = await base.GetFolderInfoAsync(id);
        return new 
        {
            Name = folderItem.Name,
            Type = folderItem.Type,
            Guid = folderItem.Guid,
            CreationTime = folderItem.CreationTime,
            Group = subject?.Group.Name,
            Teacher = subject?.TeacherId,
            Description = subject?.Description
        };
    }

    public async Task<(string, Object)> CreateAsync(string parentId, string name, User user, Dictionary<string, string>? parameters=null)
    {
        if (user.Role.Name != "Administrator")
            throw new AccessDeniedException();
        
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
        
        var (itemPath, item) = await base.CreateAsync(parentId, name, 4, user);
        var subject = new Subject
        {
            Id = item.Guid,
            GroupId = group.Id,
            Name = name,
            TeacherId = teacherId,
            Description = parameters?.ContainsKey("Description") == true ? parameters["Description"] : null
        };
        await _commonSubjectQueries.CreateAsync(subject);
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
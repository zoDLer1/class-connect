using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public class SubjectHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, Group> _commonGroupQueries;
    private CommonQueries<string, Subject> _commonSubjectQueries;
    
    public SubjectHelperService(IHostEnvironment env, Context context) : base(env, context)
    {
        _commonGroupQueries = new CommonQueries<string, Group>(_context);
        _commonSubjectQueries = new CommonQueries<string, Subject>(_context);
    }

    public async Task<(string, FolderItem)> CreateAsync(string parentId, string name, Dictionary<string, string>? parameters=null)
    {
        // Проверяем, является ли родитель папкой
        var group = await _commonGroupQueries.GetAsync(parentId, _context.Groups);
        if (group == null)
            throw new InvalidPathException();

        // Проверяем, есть ли у данной группы такой предмет
        var groupSubjects = await _context.Subjects.Where(s => s.GroupId == parentId).ToListAsync();
        var anotherSubject = groupSubjects.FirstOrDefault(g => g.Name == name);
        if (anotherSubject != null)
            throw new InvalidSubjectNameException();
        
        var (path, item) = await base.CreateAsync(parentId, name, 4);
        var subject = new Subject
        {
            Id = item.Guid,
            GroupId = group.Id,
            Name = name,
            Description = parameters?.ContainsKey("Description") == true ? parameters["Description"] : null
        };
        await _commonSubjectQueries.CreateAsync(subject);
        return (path, item);
    }

    public async override Task<FolderItem> UpdateAsync(string id, string newName)
    {
        var subject = await _commonSubjectQueries.GetAsync(id, _context.Subjects);
        if (subject == null)
            throw new ItemNotFoundException();

        // Проверяем, есть ли у данной группы такой предмет
        var groupSubjects = await _context.Subjects.Where(s => s.GroupId == subject.GroupId).ToListAsync();
        var anotherSubject = groupSubjects.FirstOrDefault(g => g.Name == newName);
        if (anotherSubject != null)
            throw new InvalidSubjectNameException();

        var item = await base.UpdateAsync(id, newName);
        subject.Name = newName;
        await _commonSubjectQueries.UpdateAsync(subject);
        return item;
    }

    public async new Task DeleteAsync(string id)
    {
        await _commonSubjectQueries.DeleteAsync(id);
        var path = await base.DeleteAsync(id);
        Directory.Delete(path, true);
    }
}
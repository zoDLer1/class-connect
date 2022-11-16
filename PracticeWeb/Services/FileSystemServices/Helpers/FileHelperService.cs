using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public class FileHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, FileEntity> _commonFileQueries;
    
    public FileHelperService(
        IHostEnvironment env, 
        ServiceResolver serviceAccessor,
        Context context) : base(env, serviceAccessor, context)
    {
        _commonFileQueries = new CommonQueries<string, FileEntity>(_context);
    }

    public async Task<List<string>> HasAccessAsync(string id, User user, List<string> path)
    {
        await HasUserAccessToParentAsync(id, user, path);
        path.Add(id);
        return path;
    }

    public async Task<Object> GetAsync(string id, User user)
    {
        await HasAccessAsync(id, user, new List<string>());
        var item = await base.GetAsync(id);
        if (item.Type.Name != "File")
            throw new ItemTypeException();
        
        var fileEntity = await _commonFileQueries.GetAsync(item.Id, _context.Files);
        if (fileEntity == null)
            throw new ItemNotFoundException();

        var pathItems = await GeneratePathAsync(item.Id);
        var path = CombineWithFileSystemPath(string.Join(Path.DirectorySeparatorChar, pathItems));
        if (!IsFilePathValid(path))
            throw new ItemNotFoundException();
        
        return new PhysicalFileResult(path, fileEntity.MimeType);
    }

    public async virtual Task<Object> GetChildItemAsync(string id, User user)
    {
        await HasAccessAsync(id, user, new List<string>());
        var folderItem = await base.GetFolderInfoAsync(id);
        var fileEntity = await _commonFileQueries.GetAsync(id, _context.Files);
        return new 
        {
            Name = folderItem.Name,
            Type = folderItem.Type,
            Guid = folderItem.Guid,
            MimeType = fileEntity?.MimeType,
            CreationTime = folderItem.CreationTime,
        };
    }

    public async Task<(string, Object)> CreateAsync(string parentId, string name, User user, Dictionary<string, string>? parameters=null)
    {
        await HasUserAccessToParentAsync(parentId, user, new List<string>());
        var (itemPath, item) = await base.CreateAsync(parentId, name, 2, user);
        var fileEntity = new FileEntity
        {
            Id = item.Guid,
            Extension = Path.GetExtension(item.Name),
            MimeType = MimeTypes.GetMimeType(item.Name)
        };
        await _commonFileQueries.CreateAsync(fileEntity);
        return (itemPath, await GetChildItemAsync(item.Guid, user));
    }

    public async new Task DeleteAsync(string id)
    {
        await _commonFileQueries.DeleteAsync(id);
        var path = await base.DeleteAsync(id);
        File.Delete(path);
    }
}
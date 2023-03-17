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

    public async Task<ItemAccess> HasAccessAsync(string id, User user, List<string> path)
    {
        var access = await HasUserAccessToParentAsync(id, user, path);
        access.Path.Add(id);
        return access;
    }

    public async Task<Object> GetAsync(string id, User user)
    {
        await HasAccessAsync(id, user, new List<string>());
        var item = await base.GetAsync(id);
        if (item.Type.Id != Type.File)
            throw new ItemTypeException();

        var fileEntity = await _commonFileQueries.GetAsync(item.Id, _context.Files);
        if (fileEntity == null)
            throw new ItemNotFoundException();

        var pathItems = await GeneratePathAsync(item.Id);
        var path = CombineWithFileSystemPath(string.Join(Path.DirectorySeparatorChar, pathItems));
        if (!IsFilePathValid(path))
            throw new ItemNotFoundException();

        var filestream = new FileStream(path, FileMode.Open);
        var name = Path.GetExtension(item.Name) == fileEntity.Extension ? item.Name : item.Name + fileEntity.Extension;
        return new FileStreamResult(filestream, fileEntity.MimeType) { FileDownloadName = name };
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
            CreatorName = folderItem.CreatorName
        };
    }

    public async Task<(string, Object)> CreateAsync(string parentId, string name, User user, Dictionary<string, string>? parameters=null)
    {
        // Если пытаемся создать файл в руте
        if (parentId == _rootGuid)
            throw new InvalidPathException();

        var parent = await TryGetItemAsync(parentId);
        var access = await _serviceAccessor(parent.Type.Name).HasAccessAsync(parent.Id, user, new List<string>());

        // Если не имеет доступа и это не студент, пытающийся загрузить файл в свою работу
        if (access.Permission != Permission.Write)
            throw new AccessDeniedException();

        // Проверка допустимости типов
        if (!TypeDependence.File.Contains(parent.TypeId))
            throw new InvalidPathException();

        var (itemPath, item) = await base.CreateAsync(parentId, name, Type.File, user);
        var fileEntity = new FileEntity
        {
            Id = item.Guid,
            Extension = Path.GetExtension(item.Name),
            MimeType = MimeTypes.GetMimeType(item.Name)
        };
        await _commonFileQueries.CreateAsync(fileEntity);
        return (itemPath, await GetChildItemAsync(item.Guid, user));
    }

    public async new Task DeleteAsync(string id, User user)
    {
        var parentConnection = await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == id);
        if (parentConnection == null)
            throw new AccessDeniedException();

        var parent = await TryGetItemAsync(parentConnection.ParentId);
        var path = await base.DeleteAsync(id, user);

        if (parent.TypeId == Type.Work)
        {
            var item = await _context.WorkItems.Include(w => w.Work).FirstOrDefaultAsync(w => w.Id == id);
            if (item == null)
                throw new AccessDeniedException();

            _context.WorkItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        File.Delete(path);
    }
}

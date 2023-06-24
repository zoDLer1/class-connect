using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassConnect.Exceptions;
using ClassConnect.Models;

namespace ClassConnect.Services.FileSystemServices.Helpers;

public class FileHelperService : FileSystemQueriesHelper, IFileSystemHelper
{
    private CommonQueries<string, FileEntity> _commonFileQueries;

    public FileHelperService(IHostEnvironment env, ServiceResolver serviceAccessor, Context context)
        : base(env, serviceAccessor, context)
    {
        _commonFileQueries = new CommonQueries<string, FileEntity>(_context);
    }

    public async Task<ItemAccess> HasAccessAsync(string id, User user, List<string> path)
    {
        var access = await HasUserAccessToParentAsync(id, user, path);
        Console.WriteLine($"file access: {access.Permission} in {id}");
        access.Path.Add(id);
        return access;
    }

    public async Task<object> GetAsync(string id, User user, Boolean asChild)
    {
        await HasAccessAsync(id, user, new List<string>());
        var item = await base.GetAsync(id);
        if (item.Type.Id != Type.File)
            throw new ItemTypeException();

        var fileEntity = await _commonFileQueries.GetAsync(item.Id, _context.Files);
        if (fileEntity == null)
            throw new ItemNotFoundException();

        if (!asChild)
        {
            var pathItems = await GeneratePathAsync(item.Id);
            var path = CombineWithFileSystemPath(
                string.Join(Path.DirectorySeparatorChar, pathItems)
            );
            if (!IsFilePathValid(path))
                throw new ItemNotFoundException();

            var filestream = new FileStream(path, FileMode.Open);
            var name =
                Path.GetExtension(item.Name) == fileEntity.Extension
                    ? item.Name
                    : item.Name + fileEntity.Extension;
            return new FileStreamResult(filestream, fileEntity.MimeType)
            {
                FileDownloadName = name
            };
        }

        var access = await HasAccessAsync(id, user, new List<string>());
        var folderItem = await base.GetFolderInfoAsync(id);
        return new
        {
            Name = folderItem.Name,
            Type = folderItem.Type,
            Guid = folderItem.Guid,
            Data = new
            {
                MimeType = fileEntity?.MimeType,
                CreationTime = folderItem.Data.CreationTime,
                CreatorName = folderItem.Data.CreatorName,
            },
            IsEditable = CanEdit(item, user, access.Permission)
        };
    }

    public async Task CheckIfCanCreateAsync(string parentId, User user)
    {
        if (parentId == _rootGuid)
            throw new InvalidPathException();

        await base.CheckIfCanCreateAsync(parentId, Type.File, user);
        var parent = await TryGetItemAsync(parentId);

        if (parent.TypeId == Type.Work && user.RoleId != UserRole.Student)
            throw new AccessDeniedException();
    }

    public async Task<(string, object)> CreateAsync(
        string parentId,
        string name,
        User user,
        Dictionary<string, object>? parameters = null
    )
    {
        await CheckIfCanCreateAsync(parentId, user);
        var (itemPath, item) = await base.CreateAsync(
            parentId,
            name.Substring(0, Math.Min(70, name.Length)),
            Type.File,
            user
        );
        var fileEntity = new FileEntity
        {
            Id = item.Guid,
            Extension = Path.GetExtension(name),
            MimeType = MimeTypes.GetMimeType(name)
        };
        await _commonFileQueries.CreateAsync(fileEntity);
        return (itemPath, await GetAsync(item.Guid, user, true));
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
            var item = await _context.WorkItems
                .Include(w => w.Work)
                .FirstOrDefaultAsync(w => w.Id == id);
            if (item == null)
                throw new AccessDeniedException();

            _context.WorkItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        File.Delete(path);
    }
}

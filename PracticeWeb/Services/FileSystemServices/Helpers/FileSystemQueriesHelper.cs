using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public abstract class FileSystemQueriesHelper
{
    protected Context _context;
    protected CommonQueries<string, Item> _common;
    protected ServiceResolver _serviceAccessor;
    protected string? _rootGuid;
    private string _fileSystemPath;
    private Regex _returnPattern = new Regex(@"\/\.\.(?![^\/])");

    public FileSystemQueriesHelper(IHostEnvironment env, ServiceResolver serviceAccessor, Context context)
    {
        _context = context;
        _common = new CommonQueries<string, Item>(_context);
        _serviceAccessor = serviceAccessor;
        _fileSystemPath = Path.Combine(env.ContentRootPath, "Filesystem");
        if (IsFolderPathValid(_fileSystemPath))
        {
            var rootPath = Directory.GetFileSystemEntries(_fileSystemPath).FirstOrDefault();
            _rootGuid = Path.GetFileName(rootPath);
        }
    }

    protected async Task<Item> TryGetItemAsync(string id)
    {
        var item = await _common.GetAsync(id, IncludeValues());
        if (item == null)
            throw new ItemNotFoundException();

        return item;
    }

    protected async Task<ItemAccess> HasUserAccessToParentAsync(string id, User user, List<string> path)
    {
        var connection = await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == id);
        if (connection == null)
            throw new InvalidPathException();
        var parent = await TryGetItemAsync(connection.ParentId);
        // Проверяем, имеет ли пользователь доступ к родителю
        Console.WriteLine($"{parent.Type.Name}: {parent.Id} – {parent.Name}");
        return await _serviceAccessor(parent.Type.Id).HasAccessAsync(parent.Id, user, path);
    }

    protected string CombineWithFileSystemPath(string path) => Path.Combine(_fileSystemPath, path);

    private bool HasReturns(string path) => _returnPattern.IsMatch(path);

    protected bool IsFilePathValid(string path) => !HasReturns(path) && File.Exists(path);

    protected bool IsFolderPathValid(string path) => !HasReturns(path) && Directory.Exists(path);

    private async Task<List<string>> GetCreatingTypesAsync(FolderItem item, User user, Permission permission)
    {
        var result = new List<string>();

        Console.WriteLine($"getting access for {item.Type.Id} with {permission} {typeof(TypeDependence).GetProperties().Length}");

        foreach (var prop in typeof(TypeDependence).GetProperties())
        {
            Console.WriteLine($"property: {prop.Name}");
            var dependence = prop.GetValue(null, null) as HashSet<Type>;
            if (dependence?.Contains(item.Type.Id) == false)
                continue;

            try
            {
                await _serviceAccessor((Type) Enum.Parse(typeof(Type), prop.Name)).CheckIfCanCreateAsync(item.Guid, user);
                result.Add(prop.Name);
            }
            catch {}
        }

        return result;
    }

    protected async Task<List<object>> MakePathAsync(List<string> ids)
    {
        var result = new List<object>();
        foreach (var id in ids)
        {
            var item = await _common.GetAsync(id, _context.Items.Include(i => i.Type));
            if (item == null)
                continue;
            result.Add(new
            {
                Name = item.Name,
                Guid = item.Id,
                Type = item.Type
            });
        }
        return result;
    }

    protected async Task<FolderItem> PrepareItemAsync(string id)
    {
        var item = await TryGetItemAsync(id);
        return new FolderItem()
        {
            Name = item.Name,
            Guid = item.Id,
            Type = item.Type,
            Data = new FolderData {
                CreationTime = item.CreationTime,
                CreatorName = $"{item.Creator.Name} {item.Creator.Surname}"
            }
        };
    }

    protected async Task<List<object>> PrepareChildrenAsync(List<string> itemIds, User user)
    {
        var children = new List<(Type Type, string Name, object Child)>();
        Console.WriteLine($"There is {itemIds.Count}");
        foreach (var id in itemIds)
        {
            Console.WriteLine($"Child item is {id}");
            try
            {
                var item = await TryGetItemAsync(id);
                try
                {
                    var child = await _serviceAccessor(item.TypeId).GetAsync(item.Id, user, true);
                    children.Add((item.TypeId, item.Name, child));
                }
                catch (AccessDeniedException)
                {
                    continue;
                }

            }
            catch (ItemNotFoundException)
            {
                continue;
            }
        }
        return children.OrderByDescending(c => c.Type).ThenBy(c => c.Name).Select(c => c.Child).ToList();
    }

    public async virtual Task CheckIfCanCreateAsync(string parentId, Type type, User user)
    {
        var parent = await TryGetItemAsync(parentId);
        var access = await _serviceAccessor(parent.Type.Id).HasAccessAsync(parent.Id, user, new List<string>());

        if (access.Permission != Permission.Write)
            throw new AccessDeniedException();

        // Проверка допустимости типов
        var prop = typeof(TypeDependence).GetProperty(type.ToString())?.GetValue(null, null) as HashSet<Type>;
        if (prop == null || !prop.Contains(parent.TypeId))
            throw new InvalidPathException();
    }

    public async virtual Task<Permission> GetPermission(int userId, string id) {
        var access = await _context.Accesses.FirstOrDefaultAsync((a) => a.UserId == userId && a.ItemId == id);
        if (access == null)
            return Permission.None;
        return access.Permission;
    }

    public async virtual Task<Item> GetAsync(string id)
    {
        return await TryGetItemAsync(id);
    }

    public async Task<FolderItem> GetFolderInfoAsync(string id)
    {
        return await PrepareItemAsync(id);
    }

    public async virtual Task<Folder> GetFolderAsync(string id, User user, Boolean asChild)
    {
        var item = await GetFolderInfoAsync(id);
        if (item.Type.Id == Type.File)
            throw new ItemTypeException();

        Console.WriteLine($"Item type name is {item.Type.Name}");
        var access = await _serviceAccessor(item.Type.Id).HasAccessAsync(id, user, new List<string>());
        var types = await GetCreatingTypesAsync(item, user, access.Permission);
        if (access.Permission == Permission.None)
            throw new AccessDeniedException();
        var children = await _context.Connections.Where(c => c.ParentId == item.Guid).Select(i => i.ChildId).ToListAsync();
        var folder = new Folder
        {
            Name = item.Name,
            Type = item.Type,
            Guid = item.Guid,
            Path = await MakePathAsync(access.Path),
            Children = asChild ? null : await PrepareChildrenAsync(children, user),
            Data = new FolderData {
                CreationTime = item.Data.CreationTime,
                CreatorName = item.Data.CreatorName
            },
            Access = types.OrderBy(t => t).ToList(),
            IsEditable = CanEdit(await TryGetItemAsync(id), user, access.Permission)
        };
        return folder;
    }

    protected async Task<object?> GetWorkData(string id, User user)
    {
        var work = await _context.Works.FirstOrDefaultAsync(w => w.Id == id);
        if (work == null)
            return null;

        var parentConnection = await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == id);
        if (parentConnection == null)
            throw new ItemNotFoundException();

        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == parentConnection.ParentId);
        if (task == null)
            throw new ItemNotFoundException();

        var items = _context.WorkItems
            .Where(w => w.WorkId == work.Id)
            .ToList()
            .Select(async w =>
            {
                var item = await _common.GetAsync(w.Id, _context.Items.Include(e => e.Type));
                var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == w.Id);
                return (item?.Name, new
                {
                    Id = w.Id,
                    Name = item?.Name,
                    Type = item?.Type,
                    MimeType = file?.MimeType,
                    IsEditable = user.Id == item?.CreatorId
                });
            })
            .Select(r => r.Result)
            .OrderBy(i => i.Name)
            .Select(i => i.Item2)
            .ToList();
        return new
        {
            Guid = work.Id,
            IsLate = work.SubmitDate > task.Until,
            IsSubmitted = work.IsSubmitted,
            Mark = work.Mark,
            Files = items
        };
    }

    protected async Task<List<string>> GeneratePathAsync(string childId)
    {
        var connection = await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == childId);;
        if (connection == null)
            return new List<string>() { childId };

        var result = await GeneratePathAsync(connection.ParentId);
        result.Add(connection.ChildId);
        return result;
    }

    private async Task<string> MakeFullPathAsync(string id)
    {
        var pathItems = await GeneratePathAsync(id);
        var path = Path.Combine(_fileSystemPath, string.Join(Path.DirectorySeparatorChar, pathItems));
        if (!(IsFolderPathValid(path) || IsFilePathValid(path)))
            throw new FolderNotFoundException();

        return path;
    }

    public async virtual Task<(string, FolderItem)> CreateAsync(string parentId, string name, Type type, User user)
    {
        var parent = await TryGetItemAsync(parentId);
        var item = new Item
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = type,
            Name = name,
            CreationTime = DateTime.Now,
            CreatorId = user.Id
        };
        var connection = new Connection
        {
            ParentId = parentId,
            ChildId = item.Id,
        };

        var path = await MakeFullPathAsync(parent.Id);
        Console.WriteLine($"path: {path}");
        if (!IsFolderPathValid(path))
            throw new FolderNotFoundException();

        await _common.CreateAsync(item);
        await _context.Connections.AddAsync(connection);
        await _context.SaveChangesAsync();
        return (Path.Combine(path, item.Id), await PrepareItemAsync(item.Id));
    }

    protected Boolean CanEdit(Item item, User user, Permission permission) =>
        !(permission < Permission.Write || !(user.Role.Id == UserRole.Administrator || item.CreatorId == user.Id));

    public async virtual Task<FolderItem> UpdateAsync(string id, string newName, User user)
    {
        var access = await HasUserAccessToParentAsync(id, user, new List<string>());
        if (access.Permission < Permission.Write)
            throw new AccessDeniedException();

        var item = await TryGetItemAsync(id);
        // Если пользователь не администратор и не создатель файла
        if (user.Role.Id != UserRole.Administrator && item.CreatorId != user.Id)
            throw new AccessDeniedException();

        item.Name = newName;
        await _common.UpdateAsync(item);
        return await PrepareItemAsync(item.Id);
    }

    public async virtual Task<FolderItem> UpdateTypeAsync(string id, Type newType, User user)
    {
        var access = await HasUserAccessToParentAsync(id, user, new List<string>());
        var item = await TryGetItemAsync(id);

        // Если доступ ниже записи или если пользователь не администратор и не создатель файла
        if (access.Permission < Permission.Write || !(user.Role.Id == UserRole.Administrator || item.CreatorId == user.Id))
            throw new AccessDeniedException();

        // Считаем количество детей, которые являются работами
        var workChildrenCount =  _context
            .Connections
            .Include(c => c.Child)
            .Where(c => c.ParentId == id && c.Child.TypeId == Type.Work)
            .Count();

        Console.WriteLine($"the item type is {item.TypeId} and children count is {workChildrenCount} {item.TypeId != Type.Folder} {(item.TypeId == Type.Task && workChildrenCount > 0)}");

        // Изменяемый объект должен быть папкой или заданием, в которое ещё не сдали работы
        if (item.TypeId != Type.Folder && !(item.TypeId == Type.Task && workChildrenCount == 0))
            throw new ItemTypeException();

        if (newType != Type.Folder && newType != Type.Task)
            throw new ItemTypeException();

        item.TypeId = newType;
        await _common.UpdateAsync(item);
        return await PrepareItemAsync(item.Id);
    }

    private async Task RemoveConnectionRecursively(string parentId)
    {
        var parent = await TryGetItemAsync(parentId);
        var connections = await _context.Connections.Where(c => c.ParentId == parentId).ToListAsync();
        foreach (var connection in connections)
        {
            var child = await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == connection.ChildId);
            if (child == null || child.ParentId != parentId)
                continue;
            _context.Connections.Remove(child);
            await _context.SaveChangesAsync();
            await RemoveConnectionRecursively(connection.ChildId);
        }

        if (parent.TypeId == Type.File)
        {
            var file = await _context.Files.FirstOrDefaultAsync((e) => e.Id == parent.Id);
            if (file != null)
            {
                _context.Files.Remove(file);
                await _context.SaveChangesAsync();
            }
        }

        await _common.DeleteAsync(parent.Id);
    }

    public async virtual Task<string> DeleteAsync(string id, User user)
    {
        var access = await HasUserAccessToParentAsync(id, user, new List<string>());
        var parentConnection = await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == id);
        if (parentConnection == null)
            throw new FolderNotFoundException();

        var parent = await TryGetItemAsync(parentConnection.ParentId);
        Console.WriteLine($"the user {user.Id} with role {user.RoleId} has access {access.Permission}. can write {(user.RoleId == UserRole.Student && parent.TypeId == Type.Work && access.Permission == Permission.Read)} in {parent.TypeId}");

        if (access.Permission < Permission.Write)
            throw new AccessDeniedException();

        var item = await TryGetItemAsync(id);
        // Если пользователь не администратор и не создатель файла
        if (user.Role.Id != UserRole.Administrator && item.CreatorId != user.Id)
            throw new AccessDeniedException();

        var path = await MakeFullPathAsync(item.Id);
        await RemoveConnectionRecursively(item.Id);
        return path;
    }

    private IQueryable<Item> IncludeValues() =>
        _context.Items
            .Include(e => e.Type)
            .Include(e => e.Creator);
}

using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices;

public class FileSystemService : IFileSystemService
{
    public string? RootGuid { get; private set; }
    private string _fileSystemPath;
    protected Context _context;
    private CommonQueries<string, Item> _commonItemQueries;
    private CommonQueries<string, Work> _commonWorkQueries;
    private ServiceResolver _serviceAccessor;
    private Regex ReturnPattern = new Regex(@"\/\.\.(?![^\/])");

    public FileSystemService(
        IHostEnvironment env,
        Context context,
        ServiceResolver serviceAccessor)
    {
        _context = context;
        _fileSystemPath = Path.Combine(env.ContentRootPath, "Filesystem");
        if (Directory.Exists(_fileSystemPath))
        {
            var rootPath = Directory.GetFileSystemEntries(_fileSystemPath).FirstOrDefault();
            RootGuid = Path.GetFileName(rootPath);
        }
        _serviceAccessor = serviceAccessor;
        _commonItemQueries = new CommonQueries<string, Item>(_context);
        _commonWorkQueries = new CommonQueries<string, Work>(_context);
    }

    private void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    private async Task<string> CreateRootAsync(string rootGuid)
    {
        var path = Path.Combine(_fileSystemPath, rootGuid);
        CreateDirectory(path);
        if (await _commonItemQueries.GetAsync(rootGuid, _context.Items) == null)
        {
            var item = new Item
            {
                Id = rootGuid,
                TypeId = Type.Folder,
                Name = "Группы",
                CreationTime = DateTime.Now,
                CreatorId = 1,
            };
            await _commonItemQueries.CreateAsync(item);
            var adminAccess = new Access {
                Permission = Permission.Write,
                ItemId = rootGuid,
                UserId = 1,
            };
            _context.Accesses.Add(adminAccess);
            await _context.SaveChangesAsync();
        }
        return rootGuid;
    }

    private async Task CreateFileSystemAsync()
    {
        CreateDirectory(_fileSystemPath);
        if (RootGuid != null)
            await CreateRootAsync(RootGuid);
        else
            RootGuid = await CreateRootAsync(Guid.NewGuid().ToString());

        var groups = _context.Groups.ToList();
        foreach (var group in groups)
        {
            var connection = new Connection
            {
                ParentId = RootGuid,
                ChildId = group.Id,
            };
            await _context.Connections.AddAsync(connection);
            var teacherAccess = new Access {
                Permission = Permission.Write,
                ItemId = group.Id,
                UserId = group.TeacherId,
            };
            _context.Accesses.Add(teacherAccess);
            await _context.SaveChangesAsync();
            CreateDirectory(Path.Combine(_fileSystemPath, RootGuid, group.Id));
        }
    }

    public async Task RecreateFileSystemAsync()
    {
        if (Directory.Exists(_fileSystemPath))
            Directory.Delete(_fileSystemPath, true);
        await CreateFileSystemAsync();
    }

    private async Task CreateFileSystemIfNotExistsAsync()
    {
        if (await _context.Database.EnsureCreatedAsync())
            await RecreateFileSystemAsync();
        else if (!Directory.Exists(_fileSystemPath))
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
            await RecreateFileSystemAsync();
        }
    }

    private async Task<Item> TryGetItemAsync(string id)
    {
        await CreateFileSystemIfNotExistsAsync();

        var item = await _commonItemQueries.GetAsync(id, _context.Items.Include(c => c.Type));
        if (item == null)
            throw new ItemNotFoundException();

        return item;
    }

    public async Task<object> GetObjectAsync(string id, User user)
    {
        await CreateFileSystemIfNotExistsAsync();
        var item = await TryGetItemAsync(id);
        return await _serviceAccessor(item.Type.Id).GetAsync(id, user, false);
    }

    public async Task<object> SubmitWork(string id, User user)
    {
        await CreateFileSystemIfNotExistsAsync();
        var item = await TryGetItemAsync(id);

        if (item.CreatorId != user.Id)
            throw new AccessDeniedException();

        var work = await _commonWorkQueries.GetAsync(id, _context.Works);
        if (work == null)
            throw new ItemNotFoundException();

        if (work.IsSubmitted)
        {
            work.IsSubmitted = false;
            work.SubmitDate = null;
        }
        else
        {
            work.IsSubmitted = true;
            work.SubmitDate = DateTime.Now;
        }

        await _commonWorkQueries.UpdateAsync(work);
        var parentConnection = await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == id);
        if (parentConnection == null)
            throw new ItemNotFoundException();

        return await _serviceAccessor(Type.Task).GetAsync(parentConnection.ParentId, user, false);
    }

    public async Task<object> MarkWork(string id, int? mark, User user)
    {
        await CreateFileSystemIfNotExistsAsync();
        var item = await TryGetItemAsync(id);

        var parentConnection = await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == id);

        if (parentConnection == null)
            throw new ItemNotFoundException();

        var parent = await _commonItemQueries.GetAsync(parentConnection.ParentId, _context.Items);

        if (parent == null)
            throw new ItemNotFoundException();

        // Поставить оценку может либо создатель задания, либо администратор
        if (!(parent.CreatorId == user.Id || user.RoleId == UserRole.Administrator))
            throw new AccessDeniedException();

        var work = await _commonWorkQueries.GetAsync(id, _context.Works);
        if (work == null)
            throw new ItemNotFoundException();

        if (!work.IsSubmitted)
            throw new ItemTypeException();

        if (mark == null)
            work.IsSubmitted = false;
        else
            work.Mark = mark;
        await _commonWorkQueries.UpdateAsync(work);
        return await _serviceAccessor(Type.Task).GetAsync(parentConnection.ParentId, user, false);
    }

    public async Task<object> CreateWorkAsync(string parentId, string name, IFormFile file, User user)
    {
        await CreateFileSystemIfNotExistsAsync();
        var id = string.Empty;
        var workConnection = await _context.Connections.Include(c => c.Child).FirstOrDefaultAsync(c => c.ParentId == parentId && c.Child.CreatorId == user.Id);

        Console.WriteLine($"does work exist?: {workConnection != null} parent id — {parentId}");

        // Если работа ещё не была создана, то создаём
        if (workConnection == null)
        {
            var (path, item) = await _serviceAccessor(Type.Work).CreateAsync(parentId, name, user, null);
            Console.WriteLine($"creating directory in {path}");
            CreateDirectory(path);
            id = Path.GetFileName(path);
        }
        else
        {
            var workEntity = await _commonWorkQueries.GetAsync(workConnection.ChildId, _context.Works);
            if (workEntity == null)
                throw new ItemNotFoundException();

            if (workEntity.IsSubmitted)
                throw new ItemTypeException();

            id = workEntity.Id;
        }

        var child = await TryGetItemAsync(id);
        var result = await CreateFileAsync(id, file, user);
        var workItemId = (string) (result.GetType().GetProperty("Guid")?.GetValue(result) ?? "");
        var workItem = new WorkItem
        {
            Id = workItemId,
            WorkId = id
        };
        await _context.WorkItems.AddAsync(workItem);
        await _context.SaveChangesAsync();
        return await _serviceAccessor(Type.Task).GetAsync(parentId, user, false);
    }

    public async Task<object> CreateFileAsync(string parentId, IFormFile file, User user)
    {
        await CreateFileSystemIfNotExistsAsync();
        Console.WriteLine($"UPLOADING FILE {parentId}");
        var (path, item) = await _serviceAccessor(Type.File).CreateAsync(parentId, file.FileName.Trim(), user);
        Console.WriteLine($"UPLOADING FILE {path}: {parentId}");
        using (var fileStream = new FileStream(path, FileMode.Create))
            await file.CopyToAsync(fileStream);
        return item;
    }

    public async Task<object> CreateFolderAsync(string parentId, string name, Type type, User user, Dictionary<string, object>? parameters)
    {
        if (type == Type.File)
            throw new KeyNotFoundException();
        await CreateFileSystemIfNotExistsAsync();
        var parent = await TryGetItemAsync(parentId);
        var (path, item) = await _serviceAccessor(type).CreateAsync(parentId, name.Trim(), user, parameters);
        CreateDirectory(path);
        return item;
    }

    public async Task RenameAsync(string id, string newName, User user)
    {
        var item = await TryGetItemAsync(id);
        var newItem = await _serviceAccessor(item.Type.Id).UpdateAsync(id, newName.Trim(), user);
    }

    public async Task UpdateTypeAsync(string id, Type newType, User user)
    {
        var item = await TryGetItemAsync(id);
        var newItem = await _serviceAccessor(item.Type.Id).UpdateTypeAsync(id, newType, user);
    }

    public async Task RemoveAsync(string id, User user)
    {
        var item = await TryGetItemAsync(id);
        await _serviceAccessor(item.Type.Id).DeleteAsync(id, user);
    }
}

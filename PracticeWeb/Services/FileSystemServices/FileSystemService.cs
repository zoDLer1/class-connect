using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
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
    private ServiceResolver _serviceAccessor;
    private Regex ReturnPattern = new Regex(@"\/\.\.(?![^\/])");

    public FileSystemService(
        IHostEnvironment env,
        Context context,
        ServiceResolver serviceAccessor)
    {
        _context = context;
        _fileSystemPath = Path.Combine(env.ContentRootPath, "Filesystem");
        if (IsFolderPathValid(_fileSystemPath))
        {
            var rootPath = Directory.GetFileSystemEntries(_fileSystemPath).FirstOrDefault();
            RootGuid = Path.GetFileName(rootPath);
        }
        _serviceAccessor = serviceAccessor;
        _commonItemQueries = new CommonQueries<string, Item>(_context);
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
                TypeId = 1,
                Name = "Корень",
                CreationTime = DateTime.Now,
                CreatorId = 1,
            };
            await _commonItemQueries.CreateAsync(item);
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

    private bool HasReturns(string path) => ReturnPattern.IsMatch(path);

    private bool IsFolderPathValid(string path) => !HasReturns(path) && Directory.Exists(path);

    public async Task<Item> TryGetItemAsync(string id)
    {
        await CreateFileSystemIfNotExistsAsync();

        var item = await _commonItemQueries.GetAsync(id, _context.Items.Include(c => c.Type));
        if (item == null)
            throw new ItemNotFoundException();
        
        return item;
    }

    private async Task<List<string>> GeneratePathAsync(string childId)
    {
        var connection = await _context.Connections.FirstOrDefaultAsync(c => c.ChildId == childId);;
        if (connection == null)
            return new List<string>() { childId };

        var result = await GeneratePathAsync(connection.ParentId);
        result.Add(connection.ChildId);
        return result;
    }

    public async Task<FileResult> GetFileAsync(string id, User user)
    {
        await CreateFileSystemIfNotExistsAsync();
        var file = await _serviceAccessor("File").GetAsync(id, user) as PhysicalFileResult;
        
        if (file != null)
            return file;
        throw new ItemNotFoundException();
    }

    public async Task<Object> GetObjectAsync(string id, User user)
    {
        await CreateFileSystemIfNotExistsAsync();
        var item = await TryGetItemAsync(id);
        return await _serviceAccessor(item.Type.Name).GetAsync(id, user);
    }

    public async Task<Object> CreateWorkAsync(string parentId, string name, string type, IFormFile file, User user)
    {
        await CreateFileSystemIfNotExistsAsync();
        var parent = await TryGetItemAsync(parentId);
        var (path, item) = await _serviceAccessor("Work").CreateAsync(parentId, name, user, null);
        CreateDirectory(path);
        var id = Path.GetFileName(path);
        var child = await TryGetItemAsync(id);
        var result = await CreateFileAsync(id, file, user);
        return result;
    }

    public async Task<Object> CreateFileAsync(string parentId, IFormFile file, User user)
    {
        await CreateFileSystemIfNotExistsAsync();
        var (path, item) = await _serviceAccessor("File").CreateAsync(parentId, file.FileName, user);
        using (var fileStream = new FileStream(path, FileMode.Create))
            await file.CopyToAsync(fileStream);
        return item;
    }

    public async Task<Object> CreateFolderAsync(string parentId, string name, string type, User user, Dictionary<string, string>? parameters)
    {
        await CreateFileSystemIfNotExistsAsync();
        var parent = await TryGetItemAsync(parentId);
        var (path, item) = await _serviceAccessor(type).CreateAsync(parentId, name, user, parameters);
        CreateDirectory(path);
        return item;
    }

    public async Task RenameAsync(string id, string newName, User user)
    {
        var item = await TryGetItemAsync(id);
        var newItem = await _serviceAccessor(item.Type.Name).UpdateAsync(id, newName, user);
    }

    public async Task UpdateTypeAsync(string id, string newType, User user)
    {
        var item = await TryGetItemAsync(id);
        var newItem = await _serviceAccessor(item.Type.Name).UpdateTypeAsync(id, newType, user);
    }

    public async Task RemoveAsync(string id)
    {
        var item = await TryGetItemAsync(id);
        await _serviceAccessor(item.Type.Name).DeleteAsync(id);
    }
}
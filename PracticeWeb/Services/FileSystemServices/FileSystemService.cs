using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices;

public class FileSystemService : IFileSystemService
{
    public string? RootId { get; private set; }
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
            RootId = Path.GetFileName(rootPath);
        }
        _serviceAccessor = serviceAccessor;
        _commonItemQueries = new CommonQueries<string, Item>(_context);
    }

    private void CreateDirectory(string path) 
    {
        Directory.CreateDirectory(path);
    }

    private async Task<string> CreateRoot()
    {
        var rootGuid = Guid.NewGuid().ToString();
        var path = Path.Combine(_fileSystemPath, rootGuid);
        CreateDirectory(path);
        var item = new Item 
        {
            Id = rootGuid,
            TypeId = 1,
            Name = "Корень",
            CreationTime = DateTime.Now
        };
        await _commonItemQueries.CreateAsync(item);
        return rootGuid;
    }

    private async Task CreateFileSystemAsync()
    {
        CreateDirectory(_fileSystemPath);
        var rootGuid = await CreateRoot();
        
        var groups = _context.Groups.ToList();
        foreach (var group in groups)
        {
            var connection = new Connection
            {
                ParentId = rootGuid,
                ChildId = group.Id,
            };
            await _context.Connections.AddAsync(connection);
            await _context.SaveChangesAsync();
            CreateDirectory(Path.Combine(_fileSystemPath, rootGuid, group.Id));
        }
    }

    private async Task CreateFileSystemIfNotExistsAsync() 
    {
        if (!Directory.Exists(_fileSystemPath))
            await CreateFileSystemAsync();
    }

    private bool HasReturns(string path) => ReturnPattern.IsMatch(path);

    private bool IsFilePathValid(string path) => !HasReturns(path) && File.Exists(path);

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

    private async Task<List<string>> MakePathFromNames(List<string> ids)
    {
        var result = new List<string>();
        foreach (var id in ids)
        {
            var item = await _commonItemQueries.GetAsync(id, _context.Items);
            if (item == null)
                continue;
            result.Add(item.Name);
        }
        return result;
    }

    private async Task<FolderItem> PrepareItemAsync(string id)
    {
        var item = await TryGetItemAsync(id);
        var fullPath = await GeneratePathAsync(item.Id);
        var path = string.Join(Path.DirectorySeparatorChar, fullPath);
        var file = await _context.Files.FirstOrDefaultAsync(e => e.Id == item.Id);
        return new FolderItem() 
        {
            Name = item.Name,
            Path = await MakePathFromNames(fullPath),
            Guid = item.Id,
            Type = item.Type,
            MimeType = file?.MimeType,
            CreationTime = item.CreationTime,
            CreatorName = "testName",
        };
    }

    private async Task<List<FolderItem>> PrepareItemsAsync(List<string> itemIds)
    {
        var result = new List<FolderItem>();
        foreach (var id in itemIds)
        {
            try 
            {
                var pripared = await PrepareItemAsync(id);
                result.Add(pripared);
            }
            catch (ItemNotFoundException)
            {
                continue;
            }
        }
        return result.OrderBy(i => i.Name).ThenBy(i => i.CreationTime).ToList();
    }

    public async Task<FileResult> GetFileAsync(string id)
    {
        await CreateFileSystemIfNotExistsAsync();
        var item = await _serviceAccessor("File").GetAsync(id);
        if (item.Type.Name != "File")
            throw new ItemTypeException();
        
        var fileEntity = await _context.Files.FirstOrDefaultAsync(f => f.Id == item.Guid);
        if (fileEntity == null)
            throw new ItemNotFoundException();
        
        var pathItems = await GeneratePathAsync(item.Guid);
        var path = Path.Combine(_fileSystemPath, string.Join(Path.DirectorySeparatorChar, pathItems));
        if (!IsFilePathValid(path))
            throw new ItemNotFoundException();
        
        return new PhysicalFileResult(path, fileEntity.MimeType);
    }

    public async Task<Folder> GetFolderInfoAsync(string id)
    {
        await CreateFileSystemIfNotExistsAsync();
        var item = await _serviceAccessor("Folder").GetAsync(id);
        if (item.Type.Name == "File")
            throw new ItemTypeException();
        
        var items = await _context.Connections.Where(c => c.ParentId == item.Guid).ToListAsync();
        var pathItems = await GeneratePathAsync(item.Guid);
        var folder = new Folder 
        {
            Name = item.Name, 
            Type = item.Type,
            Path = await MakePathFromNames(pathItems),
            RealPath = pathItems,
            Guid = item.Guid,
            Items = await PrepareItemsAsync(items.Select(i => i.ChildId).ToList()),
            CreationTime = item.CreationTime,
            CreatorName = "testName",
        };
        return folder;
    }

    public async Task<FolderItem> CreateFileAsync(string parentId, IFormFile file)
    {
        await CreateFileSystemIfNotExistsAsync();
        var (path, item) = await _serviceAccessor("File").CreateAsync(parentId, file.FileName);
        using (var fileStream = new FileStream(Path.Combine(path, item.Guid), FileMode.Create))
            await file.CopyToAsync(fileStream);
        return item;
    }

    public async Task<FolderItem> CreateFolderAsync(string parentId, string name, string type, Dictionary<string, string>? parameters)
    {
        await CreateFileSystemIfNotExistsAsync();
        var parent = await TryGetItemAsync(parentId);
        var (path, item) = await _serviceAccessor(type).CreateAsync(parentId, name, parameters);
        CreateDirectory(Path.Combine(path, item.Guid));
        return item;
    }

    public async Task RenameAsync(string id, string newName)
    {
        var item = await TryGetItemAsync(id);
        var newItem = await _serviceAccessor(item.Type.Name).UpdateAsync(id, newName);
    }

    public async Task UpdateTypeAsync(string id, string newType)
    {
        var item = await TryGetItemAsync(id);
        var newItem = await _serviceAccessor(item.Type.Name).UpdateTypeAsync(id, newType);
    }

    public async Task RemoveAsync(string id)
    {
        var item = await TryGetItemAsync(id);
        await _serviceAccessor(item.Type.Name).DeleteAsync(id);
    }
}
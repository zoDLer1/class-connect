using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;

namespace PracticeWeb.Services.FileSystemServices.Helpers;

public abstract class FileSystemQueriesHelper
{
    protected Context _context;
    protected CommonQueries<string, Item> _common;
    private string _fileSystemPath;
    private Regex ReturnPattern = new Regex(@"\/\.\.(?![^\/])");

    public FileSystemQueriesHelper(IHostEnvironment env, Context context)
    {
        _context = context;
        _common = new CommonQueries<string, Item>(_context);
        _fileSystemPath = Path.Combine(env.ContentRootPath, "Filesystem");
    }

    protected async Task<Item> TryGetItemAsync(string id)
    {
        var item = await _common.GetAsync(id, IncludeValues());
        if (item == null)
            throw new ItemNotFoundException();

        return item;
    }

    private bool HasReturns(string path) => ReturnPattern.IsMatch(path);

    private bool IsFilePathValid(string path) => !HasReturns(path) && File.Exists(path);

    private bool IsFolderPathValid(string path) => !HasReturns(path) && Directory.Exists(path);

    protected async Task<List<string>> MakePathFromNames(List<string> ids)
    {
        var result = new List<string>();
        foreach (var id in ids)
        {
            var item = await _common.GetAsync(id, _context.Items);
            if (item == null)
                continue;
            result.Add(item.Name);
        }
        return result;
    }

    protected async Task<FolderItem> PrepareItemAsync(string id)
    {
        var item = await TryGetItemAsync(id);
        var fullPath = await GeneratePathAsync(item.Id);
        var path = string.Join(Path.DirectorySeparatorChar, fullPath);
        var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == item.Id);
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
    public async virtual Task<FolderItem> GetAsync(string id)
    {
        var item = await TryGetItemAsync(id);      
        return await PrepareItemAsync(id);
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

    public async virtual Task<(string, FolderItem)> CreateAsync(string parentId, string name, int typeId)
    {
        var parent = await TryGetItemAsync(parentId);
        var item = new Item 
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = typeId,
            Name = name,
            CreationTime = DateTime.Now
        };
        var connection = new Connection
        {
            ParentId = parentId,
            ChildId = item.Id,
        };

        var path = await MakeFullPathAsync(parent.Id);
        if (!IsFolderPathValid(path))
            throw new FolderNotFoundException();

        await _common.CreateAsync(item);
        await _context.Connections.AddAsync(connection);
        await _context.SaveChangesAsync();
        return (path, await PrepareItemAsync(item.Id));
    }

    public async virtual Task<FolderItem> UpdateAsync(string id, string newName)
    {
        var item = await TryGetItemAsync(id);        
        item.Name = newName;
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
        
        if (parent.Type.Name == "File")
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

    public async virtual Task<string> DeleteAsync(string id)
    {
        var item = await TryGetItemAsync(id);
        var path = await MakeFullPathAsync(item.Id);
        await RemoveConnectionRecursively(item.Id);
        return path;
    }

    private IQueryable<Item> IncludeValues() =>
        _context.Items
            .Include(e => e.Type);
}
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;
using PracticeWeb.Services.GroupStorageServices;
using PracticeWeb.Services.ItemStorageServices;

namespace PracticeWeb.Services.FileSystemServices;

public class FileSystemService : IFileSystemService
{
    private string _fileSystemPath;
    private IGroupStorageService _groupStorageService;
    private IItemStorageService _itemStorageService;
    private Regex ReturnPattern = new Regex(@"\/\.\.(?![^\/])");

    public FileSystemService(
        IHostEnvironment env, 
        IGroupStorageService groupStorageService, 
        IItemStorageService itemStorageService)
    {
        _fileSystemPath = Path.Combine(env.ContentRootPath, "Filesystem");
        _groupStorageService = groupStorageService;
        _itemStorageService = itemStorageService;
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
        await _itemStorageService.CreateAsync(item);
        return rootGuid;
    }

    private async Task CreateFileSystemAsync()
    {
        CreateDirectory(_fileSystemPath);
        var rootGuid = await CreateRoot();
        _fileSystemPath = Path.Combine(_fileSystemPath, rootGuid);
        
        var groups = await _groupStorageService.GetAllAsync();
        foreach (var group in groups)
        {
            var connection = new Connection
            {
                ParentId = rootGuid,
                ChildId = group.Id,
            };
            await _itemStorageService.CreateConnectionAsync(connection);
            CreateDirectory(Path.Combine(_fileSystemPath, group.Id));
        }
    }

    private async Task CreateFileSystemIfNotExistsAsync() 
    {
        if (!Directory.Exists(_fileSystemPath))
            await CreateFileSystemAsync();
    }

    private string MakePath(string path) => Path.Combine(_fileSystemPath, path);

    private bool HasReturns(string path) => ReturnPattern.IsMatch(path);

    private bool IsPathValidForFile(string path) => HasReturns(path) || !File.Exists(path);

    private bool IsPathValidForFolder(string path) => HasReturns(path) || !Directory.Exists(path);

    private async Task<string> PreparePathForFileAsync(string path)
    {
        await CreateFileSystemIfNotExistsAsync();

        var fullPath = MakePath(path);
        if (IsPathValidForFile(fullPath))
            throw new PracticeWeb.Exceptions.FileNotFoundException();

        return fullPath;
    }

    private async Task<string> PreparePathForFolderAsync(string path)
    {
        await CreateFileSystemIfNotExistsAsync();

        var fullPath = MakePath(path);
        if (IsPathValidForFolder(fullPath))
            throw new FolderNotFoundException();

        return fullPath;
    }

    public async Task<FileResult> GetFileAsync(string path)
    {
        return await Task.Run(async () => 
        {
            var fullPath = await PreparePathForFileAsync(path);
            var name = Path.GetFileName(fullPath);
            var type = MimeTypes.GetMimeType(fullPath);
            return new PhysicalFileResult(fullPath, type);
        });
    }

    private async Task<List<string>> MakeFullPathAsync(string childId)
    {
        var connection = await _itemStorageService.GetConnectionByChildAsync(childId);
        if (connection == null)
            return new List<string>() { childId };

        var result = await MakeFullPathAsync(connection.ParentId);
        result.Add(connection.ChildId);
        return result;
    }

    private async Task<string> ParsePath(List<string> ids)
    {
        var result = new List<string>();
        Console.WriteLine(ids.Count());
        foreach (var id in ids)
        {
            var item = await _itemStorageService.GetAsync(id);
            if (item == null)
                continue;
            result.Add(item.Name);
        }
        return string.Join(Path.DirectorySeparatorChar, result);
    }

    private async Task<List<FolderItem>> PrepareItemsAsync(List<string> itemIds)
    {
        var result = new List<FolderItem>();
        foreach (var id in itemIds)
        {
            var item = await _itemStorageService.GetAsync(id);
            if (item == null)
                continue;
            var fullPath = await MakeFullPathAsync(item.Id);
            var path = string.Join(Path.DirectorySeparatorChar, fullPath);
            result.Add(new FolderItem() 
            {
                Name = item.Name,
                Path = await ParsePath(fullPath),
                Guid = item.Id,
                Type = item.Type,
                MimeType = (item.Type.Name == "File" ? MimeTypes.GetMimeType(path) : null),
                CreationTime = item.CreationTime,
                CreatorName = "testName",
            });
        }
        return result;
    }

    public async Task<Folder> GetFolderInfoAsync(string id)
    {
        await CreateFileSystemIfNotExistsAsync();

        var item = await _itemStorageService.GetAsync(id);
        if (item == null)
            throw new ItemNotFoundException();

        var items = await _itemStorageService.GetConnectionsByParentAsync(item.Id);
        var fullPath = await MakeFullPathAsync(item.Id);
        var folder = new Folder 
        {
            Name = item.Name, 
            Type = item.Type,
            Path = await ParsePath(fullPath),
            RealPath = string.Join(Path.DirectorySeparatorChar, fullPath),
            Guid = item.Id,
            Items = await PrepareItemsAsync(items.Select(i => i.ChildId).ToList()),
            CreationTime = item.CreationTime,
            CreatorName = "testName",
        };
        return folder;
    }

    public async Task CreateFileAsync(string path, IFormFile file)
    {
        await Task.Run(async () => 
        {
            var fullPath = await PreparePathForFolderAsync(path);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            using (var fileStream = new FileStream(Path.Combine(fullPath, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            } 
        });
    }

    public async Task CreateFolderAsync(string parentId, string name)
    {
        await Task.Run(async () => 
        {
            var item = new Item 
            {
                Id = Guid.NewGuid().ToString(),
                TypeId = 1,
                Name = name,
                CreationTime = DateTime.Now
            };
            var connection = new Connection
            {
                ParentId = parentId,
                ChildId = item.Id,
            };
            await _itemStorageService.CreateAsync(item);
            await _itemStorageService.CreateConnectionAsync(connection);

            var pathItems = await MakeFullPathAsync(item.Id);
            var path = string.Join(Path.DirectorySeparatorChar, pathItems.Take(pathItems.Count() - 1));
            var fullPath = await PreparePathForFolderAsync(path);
            CreateDirectory(Path.Combine(fullPath, name));
        });
    }

    public async Task RenameAsync(string id, string newName)
    {
        await Task.Run(async () => 
        {
            var item = await _itemStorageService.GetAsync(id);
            if (item == null)
                throw new ItemNotFoundException();
            item.Name = newName;
            await _itemStorageService.UpdateAsync(id, item);
        });
    }

    public async Task RemoveFileAsync(string path)
    {
        await Task.Run(async () => 
        {
            var fullPath = await PreparePathForFileAsync(path);
            File.Delete(fullPath);
        });
    }

    public async Task RemoveFolder(string path)
    {
        await Task.Run(async () => 
        {
            var fullPath = await PreparePathForFolderAsync(path);
            Directory.Delete(fullPath, true);
        });
    }
}
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
        
        var groups = await _groupStorageService.GetAllAsync();
        foreach (var group in groups)
        {
            var connection = new Connection
            {
                ParentId = rootGuid,
                ChildId = group.Id,
            };
            await _itemStorageService.CreateConnectionAsync(connection);
            CreateDirectory(Path.Combine(_fileSystemPath, rootGuid, group.Id));
        }
    }

    private async Task CreateFileSystemIfNotExistsAsync() 
    {
        if (!Directory.Exists(_fileSystemPath))
            await CreateFileSystemAsync();
    }

    private string MakeFullPath(string path) => Path.Combine(_fileSystemPath, path);

    private bool HasReturns(string path) => ReturnPattern.IsMatch(path);

    private bool IsFilePathValid(string path) => !HasReturns(path) || File.Exists(path);

    private bool IsFolderPathValid(string path) => !HasReturns(path) || Directory.Exists(path);

    private async Task<string> PreparePathForFileAsync(string path)
    {
        await CreateFileSystemIfNotExistsAsync();

        var fullPath = MakeFullPath(path);
        if (!IsFilePathValid(fullPath))
            throw new PracticeWeb.Exceptions.FileNotFoundException();

        return fullPath;
    }

    private async Task<string> PreparePathForFolderAsync(string path)
    {
        await CreateFileSystemIfNotExistsAsync();

        var fullPath = MakeFullPath(path);
        if (!IsFolderPathValid(fullPath))
            throw new FolderNotFoundException();

        return fullPath;
    }

    public async Task<Item> TryGetItemAsync(string id)
    {
        await CreateFileSystemIfNotExistsAsync();

        var item = await _itemStorageService.GetAsync(id);
        if (item == null)
            throw new ItemNotFoundException();
        
        return item;
    }

    private async Task<List<string>> GeneratePathAsync(string childId)
    {
        var connection = await _itemStorageService.GetConnectionByChildAsync(childId);
        if (connection == null)
            return new List<string>() { childId };

        var result = await GeneratePathAsync(connection.ParentId);
        result.Add(connection.ChildId);
        return result;
    }

    private async Task<string> MakePathFromNames(List<string> ids)
    {
        var result = new List<string>();
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
            var fullPath = await GeneratePathAsync(item.Id);
            var path = string.Join(Path.DirectorySeparatorChar, fullPath);
            var file = await _itemStorageService.GetFileAsync(item.Id);
            result.Add(new FolderItem() 
            {
                Name = item.Name,
                Path = await MakePathFromNames(fullPath),
                Guid = item.Id,
                Type = item.Type,
                MimeType = file?.MimeType,
                CreationTime = item.CreationTime,
                CreatorName = "testName",
            });
        }
        return result;
    }

    public async Task<FileResult> GetFileAsync(string id)
    {
        var item = await TryGetItemAsync(id);
        if (item.Type.Name != "File")
            throw new ItemTypeException();
        
        var fileEntity = await _itemStorageService.GetFileAsync(item.Id);
        if (fileEntity == null)
            throw new ItemNotFoundException();
        
        var pathItems = await GeneratePathAsync(item.Id);
        var path = Path.Combine(_fileSystemPath, string.Join(Path.DirectorySeparatorChar, pathItems));
        return new PhysicalFileResult(path, fileEntity.MimeType);
    }

    public async Task<Folder> GetFolderInfoAsync(string id)
    {
        var item = await TryGetItemAsync(id);
        if (item.Type.Name == "File")
            throw new ItemTypeException();
        var items = await _itemStorageService.GetConnectionsByParentAsync(item.Id);
        var pathItems = await GeneratePathAsync(item.Id);
        var folder = new Folder 
        {
            Name = item.Name, 
            Type = item.Type,
            Path = await MakePathFromNames(pathItems),
            RealPath = string.Join(Path.DirectorySeparatorChar, pathItems),
            Guid = item.Id,
            Items = await PrepareItemsAsync(items.Select(i => i.ChildId).ToList()),
            CreationTime = item.CreationTime,
            CreatorName = "testName",
        };
        return folder;
    }

    public async Task CreateFileAsync(string parentId, IFormFile file)
    {
        var parent = await TryGetItemAsync(parentId);
        var item = new Item 
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = 2,
            Name = file.FileName,
            CreationTime = DateTime.Now
        };
        var connection = new Connection
        {
            ParentId = parent.Id,
            ChildId = item.Id,
        };
        var fileEntity = new FileEntity
        {
            Id = item.Id,
            Extension = Path.GetExtension(item.Name),
            MimeType = MimeTypes.GetMimeType(item.Name)
        };

        var pathItems = await GeneratePathAsync(parent.Id);
        var path = Path.Combine(_fileSystemPath, string.Join(Path.DirectorySeparatorChar, pathItems));
        if (!IsFolderPathValid(path))
            throw new FolderNotFoundException();
        
        using (var fileStream = new FileStream(Path.Combine(path, item.Id), FileMode.Create))
            await file.CopyToAsync(fileStream);

        await _itemStorageService.CreateAsync(item);
        await _itemStorageService.CreateConnectionAsync(connection);
        await _itemStorageService.CreateFileAsync(fileEntity);
    }

    public async Task CreateFolderAsync(string parentId, string name)
    {
        var parent = await TryGetItemAsync(parentId);
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

        var pathItems = await GeneratePathAsync(parent.Id);
        var path = Path.Combine(_fileSystemPath, string.Join(Path.DirectorySeparatorChar, pathItems));
        if (!IsFolderPathValid(path))
            throw new FolderNotFoundException();

        CreateDirectory(Path.Combine(path, item.Id));
        await _itemStorageService.CreateAsync(item);
        await _itemStorageService.CreateConnectionAsync(connection);
    }

    public async Task RenameAsync(string id, string newName)
    {
        var item = await TryGetItemAsync(id);
        item.Name = newName;
        await _itemStorageService.UpdateAsync(id, item);
    }

    public async Task RemoveFileAsync(string path)
    {
        var fullPath = await PreparePathForFileAsync(path);
        File.Delete(fullPath);
    }

    public async Task RemoveFolder(string path)
    {
        var fullPath = await PreparePathForFolderAsync(path);
        Directory.Delete(fullPath, true);
    }
}
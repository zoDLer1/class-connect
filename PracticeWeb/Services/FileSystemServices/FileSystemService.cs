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

    private async Task CreateFileSystemAsync()
    {
        CreateDirectory(_fileSystemPath);

        var groups = await _groupStorageService.GetAllAsync();
        foreach (var group in groups)
        {
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

    private IEnumerable<string> GetItems(string path) => 
        Directory.EnumerateFileSystemEntries(path, "*");

    private async Task<IEnumerable<FolderItem>> ParseItemsAsync(IEnumerable<string> items)
    {
        var result = new List<FolderItem>();
        foreach (var itemPath in items) 
        {
            var isFolder = System.IO.File.GetAttributes(itemPath) == FileAttributes.Directory;
            var fileName = Path.GetFileName(itemPath) ?? "";
            var guid = (isFolder ? fileName : fileName.Replace(Path.GetExtension(fileName), ""));
            var item = await _itemStorageService.GetAsync(guid);
            if (item == null)
                continue;
            result.Add(new FolderItem() 
            {
                Name = item.Name,
                Path = item.Name,
                Guid = guid,
                Type = item.Type, //(isFolder ? ItemType.Folder : ItemType.File),
                MimeType = (isFolder ? null : MimeTypes.GetMimeType(itemPath)),
                CreationTime = System.IO.File.GetCreationTime(itemPath),
                CreatorName = "testName",
            });
        }
        return result;
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

    public async Task<Folder> GetFolderInfoAsync(string path)
    {
        return await Task.Run(async () => 
        {
            var fullPath = await PreparePathForFolderAsync(path);
            var name = Path.GetFileName(fullPath) ?? "";
            var items = await ParseItemsAsync(GetItems(fullPath));
            var shortPath = fullPath.Replace(_fileSystemPath, "").TrimStart('/');
            return new Folder() 
            { 
                Name = name, 
                Path = shortPath, 
                Guid = name,
                Items = items,
                CreationTime = System.IO.File.GetCreationTime(fullPath),
                CreatorName = "testName",
            };
        });
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

    public async Task CreateFolderAsync(string path, string name)
    {
        await Task.Run(async () => 
        {
            var fullPath = await PreparePathForFolderAsync(path);
            CreateDirectory(Path.Combine(fullPath, Guid.NewGuid().ToString()));
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
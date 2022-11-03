using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Exceptions;
using PracticeWeb.Services.GroupStorageServices;

namespace PracticeWeb.Services.FileSystemServices;

public class FileSystemService : IFileSystemService
{
    private string _fileSystemPath;
    private IGroupStorageService _groupStorageService;
    private Regex ReturnPattern = new Regex(@"\/\.\.(?![^\/])");

    public FileSystemService(IHostEnvironment env, IGroupStorageService groupStorageService)
    {
        _fileSystemPath = Path.Combine(env.ContentRootPath, "Filesystem");
        _groupStorageService = groupStorageService;
    }

    private void CreateDirectory(string path) 
    {
        Directory.CreateDirectory(path);
    }

    private async Task CreateFileSystem()
    {
        CreateDirectory(_fileSystemPath);

        var groups = await _groupStorageService.GetAllAsync();
        foreach (var group in groups)
        {
            CreateDirectory(Path.Combine(_fileSystemPath, group.Id));
        }
    }

    private async Task CreateFileSystemIfNotExists() 
    {
        if (!Directory.Exists(_fileSystemPath))
            await CreateFileSystem();
    }

    private string MakePath(string path) => Path.Combine(_fileSystemPath, path);

    private bool HasReturns(string path) => ReturnPattern.IsMatch(path);

    private bool IsPathValidForFile(string path) => HasReturns(path) || !File.Exists(path);

    private bool IsPathValidForFolder(string path) => HasReturns(path) || !Directory.Exists(path);

    private async Task<string> PreparePathForFile(string path)
    {
        await CreateFileSystemIfNotExists();

        var fullPath = MakePath(path);
        if (IsPathValidForFile(fullPath))
            throw new PracticeWeb.Exceptions.FileNotFoundException();

        return fullPath;
    }

    private async Task<string> PreparePathForFolder(string path)
    {
        await CreateFileSystemIfNotExists();

        var fullPath = MakePath(path);
        if (IsPathValidForFolder(fullPath))
            throw new FolderNotFoundException();

        return fullPath;
    }

    private IEnumerable<string> GetItems(string path) => 
        Directory.EnumerateFileSystemEntries(path, "*");

    private IEnumerable<Item> ParseItems(IEnumerable<string> items)
    {
        var result = new List<Item>();
        foreach (var item in items) 
        {
            var isFolder = System.IO.File.GetAttributes(item) == FileAttributes.Directory;
            var fileName = Path.GetFileName(item) ?? "";
            result.Add(new Item() 
            {
                Name = fileName,
                Path = item.Replace(_fileSystemPath, "").TrimStart('/'),
                Guid = (isFolder ? fileName : fileName.Replace(Path.GetExtension(fileName), "")),
                Type = (isFolder ? ItemTypes.Folder : ItemTypes.File),
                MimeType = (isFolder ? null : MimeTypes.GetMimeType(item)),
                CreationTime = System.IO.File.GetCreationTime(item),
                CreatorName = "testName",
            });
        }
        return result;
    }

    public async Task<FileResult> GetFile(string path)
    {
        return await Task.Run(async () => 
        {
            var fullPath = await PreparePathForFile(path);
            var name = Path.GetFileName(fullPath);
            var type = MimeTypes.GetMimeType(fullPath);
            return new PhysicalFileResult(fullPath, type);
        });
    }

    public async Task<Folder> GetFolderInfo(string path)
    {
        return await Task.Run(async () => 
        {
            var fullPath = await PreparePathForFolder(path);
            var name = Path.GetFileName(fullPath) ?? "";
            var items = ParseItems(GetItems(fullPath));
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

    public async Task CreateFile(string path, IFormFile file)
    {
        await Task.Run(async () => 
        {
            var fullPath = await PreparePathForFolder(path);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            using (var fileStream = new FileStream(Path.Combine(fullPath, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            } 
        });
    }

    public async Task CreateFolder(string path, string name)
    {
        await Task.Run(async () => 
        {
            var fullPath = await PreparePathForFolder(path);
            CreateDirectory(Path.Combine(fullPath, Guid.NewGuid().ToString()));
        });
    }

    public async Task Rename(string path, string newName)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveFile(string path)
    {
        await Task.Run(async () => 
        {
            var fullPath = await PreparePathForFile(path);
            File.Delete(fullPath);
        });
    }

    public async Task RemoveFolder(string path)
    {
        await Task.Run(async () => 
        {
            var fullPath = await PreparePathForFolder(path);
            Directory.Delete(fullPath, true);
        });
    }
}
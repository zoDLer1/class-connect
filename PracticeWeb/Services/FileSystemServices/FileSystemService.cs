using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Exceptions;

namespace PracticeWeb.Services.FileSystemServices;

public class FileSystemService : IFileSystemService
{
    private string _fileSystemPath;
    private Regex ReturnPattern = new Regex(@"\/\.\.(?![^\/])");

    public FileSystemService(IHostEnvironment env)
    {
        _fileSystemPath = Path.Combine(env.ContentRootPath, "Filesystem");
        CreateFileSystemIfNotExists();
    }

    private void CreateDirectory(string path) 
    {
        Directory.CreateDirectory(path);
    }

    private void CreateFileSystemIfNotExists() 
    {
        if (!Directory.Exists(_fileSystemPath))
            CreateDirectory(_fileSystemPath);
    }

    private string MakePath(string path) => Path.Combine(_fileSystemPath, path);

    private bool HasReturns(string path) => ReturnPattern.IsMatch(path);

    private bool IsPathValidForFile(string path) => HasReturns(path) || !File.Exists(path);


    private bool IsPathValidForFolder(string path) => HasReturns(path) || !Directory.Exists(path);

    private string PreparePathForFile(string path)
    {
        CreateFileSystemIfNotExists();

        var fullPath = MakePath(path);
        if (IsPathValidForFile(fullPath))
            throw new PracticeWeb.Exceptions.FileNotFoundException();

        return fullPath;
    }

    private string PreparePathForFolder(string path)
    {
        CreateFileSystemIfNotExists();

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
        return await Task.Run(() => 
        {
            var fullPath = PreparePathForFile(path);
            var name = Path.GetFileName(fullPath);
            var type = MimeTypes.GetMimeType(fullPath);
            return new PhysicalFileResult(fullPath, type);
        });
    }

    public async Task<Folder> GetFolderInfo(string path)
    {
        return await Task.Run(() => 
        {
            var fullPath = PreparePathForFolder(path);
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
        var fullPath = PreparePathForFolder(path);
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        using (var fileStream = new FileStream(Path.Combine(fullPath, fileName), FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
    }

    public async Task CreateFolder(string path, string name)
    {
        await Task.Run(() => 
        {
            var fullPath = PreparePathForFolder(path);
            CreateDirectory(Path.Combine(fullPath, Guid.NewGuid().ToString()));
        });
    }

    public async Task Rename(string path, string newName)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveFile(string path)
    {
        await Task.Run(() => 
        {
            var fullPath = PreparePathForFile(path);
            File.Delete(fullPath);
        });
    }

    public async Task RemoveFolder(string path)
    {
        await Task.Run(() => 
        {
            var fullPath = PreparePathForFolder(path);
            Directory.Delete(fullPath, true);
        });
    }
}
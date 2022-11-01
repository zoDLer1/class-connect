using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Exceltions;

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

    private bool IsPathValid(string path) => HasReturns(path) || !Directory.Exists(path);

    private IEnumerable<string> GetItems(string path) => 
        Directory.EnumerateFileSystemEntries(path, "*");

    private IEnumerable<Item> ParseItems(IEnumerable<string> items)
    {
        var result = new List<Item>();
        foreach (var item in items) 
        {
            var isFolder = System.IO.File.GetAttributes(item) == FileAttributes.Directory;
            result.Add(new Item() 
            {
                Name = Path.GetFileName(item) ?? "",
                Path = item.Replace(_fileSystemPath, "").TrimStart('/'),
                Type = (isFolder ? ItemTypes.Folder : ItemTypes.File),
                CreationTime = System.IO.File.GetCreationTime(item),
            });
        }
        return result;
    }

    public async Task<FileResult> GetFile(string path)
    {
        throw new NotImplementedException();
    }

    public async Task<Folder> GetFolderInfo(string path)
    {
        return await Task.Run(() => 
        {
            CreateFileSystemIfNotExists();

            var fullPath = MakePath(path);
            if (IsPathValid(fullPath))
                throw new FolderNotFoundException();
            
            var items = ParseItems(GetItems(fullPath));
            var shortPath = fullPath.Replace(_fileSystemPath, "").TrimStart('/');

            return new Folder() 
            { 
                Name = Path.GetFileName(fullPath) ?? "", 
                Path = shortPath, 
                Items = items,
            };
        });
    }

    public async Task CreateFile(string path, IFormFile file)
    {
        CreateFileSystemIfNotExists();

        var fullPath = MakePath(path);
        if (IsPathValid(fullPath))
            throw new FolderNotFoundException();

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
            CreateFileSystemIfNotExists();

            var fullPath = MakePath(path);
            if (IsPathValid(fullPath))
                throw new FolderNotFoundException();

            CreateDirectory(Path.Combine(fullPath, Guid.NewGuid().ToString()));
        });
    }

    public async Task Rename(string path, string newName)
    {
        throw new NotImplementedException();
    }

    public async Task Remove(string path)
    {
        throw new NotImplementedException();
    }
}
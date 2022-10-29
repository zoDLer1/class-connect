using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class FilesystemController : ControllerBase
{
    private IHostEnvironment _environment;
    private string _filesystemPath;
    private static Regex ReturnPattern = new Regex(@"\/\.\.(?![^\/])");

    public FilesystemController(IHostEnvironment env)
    {
        _environment = env;
        _filesystemPath = Path.Combine(_environment.ContentRootPath, "Filesystem");
        if (!Directory.Exists(_filesystemPath))
            CreateDirectory(_filesystemPath);
    }

    private string MakePath(string path) =>
        Path.Combine(_filesystemPath, path);

    private IEnumerable<string> GetItems(string path) => 
        Directory.EnumerateFileSystemEntries(path, "*");

    private bool HasReturns(string path) => FilesystemController.ReturnPattern.IsMatch(path);

    private IEnumerable<Item> ParseItems(IEnumerable<string> items)
    {
        var result = new List<Item>();
        foreach (var item in items) 
        {
            var isFolder = System.IO.File.GetAttributes(item) == FileAttributes.Directory;
            result.Add(new Item() 
            {
                Name = Path.GetFileName(item) ?? "",
                Type = (isFolder ? ItemTypes.Folder : ItemTypes.File),
                CreationTime = System.IO.File.GetCreationTime(item),
            });
        }
        return result;
    }

    private void CreateDirectory(string path) 
    {
        Directory.CreateDirectory(path);
    }

    [HttpGet]
    public async Task<Folder?> GetFolderAsync(string? path)
    {
        return await Task.Run(() => {
            if (path == null)
                path = "";
            if (path.StartsWith("/")) 
                path = path.TrimStart('/');
            if (path.EndsWith("/")) 
                path = path.TrimEnd('/');

            if (!Directory.Exists(_filesystemPath))
                CreateDirectory(_filesystemPath);

            var currentPath = MakePath(path);
            if (HasReturns(currentPath) || !Directory.Exists(currentPath))
                currentPath = _filesystemPath;

            var items = ParseItems(GetItems(currentPath));
            if (currentPath == _filesystemPath)
                return new Folder() 
                { 
                    Name = "",
                    Path = "/", 
                    Items = items,
                };

            var shortPath = currentPath.Replace(_filesystemPath, "");
            return new Folder() 
            { 
                Name = Path.GetFileName(currentPath) ?? "", 
                Path = shortPath, 
                Items = items,
            };
        });
    }
}

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;
using PracticeWeb.Services.FileSystemServices;
using PracticeWeb.Services.GroupStorageServices;
using PracticeWeb.Services.ItemStorageServices;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class FileSystemController : ControllerBase
{
    private IFileSystemService _fileSystemService;
    private IGroupStorageService _groupStorageService;
    private IItemStorageService _itemStorageService;

    public FileSystemController(
        IFileSystemService fileSystemService, 
        IGroupStorageService groupStorageService,
        IItemStorageService itemStorageService)
    {
        _fileSystemService = fileSystemService;
        _groupStorageService = groupStorageService;
        _itemStorageService = itemStorageService;
    }

    private void CreateDirectory(string path) 
    {
        Directory.CreateDirectory(path);
    }

    private string PreparePath(string? path)
    {
        if (path == null)
            throw new NullReferenceException();
        
        if (path.StartsWith("/")) 
            path = path.TrimStart('/');
        if (path.EndsWith("/")) 
            path = path.TrimEnd('/');
        return path;
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

    private async Task<Folder> GetFolderInfoAsync(string id)
    {
        var item = await _itemStorageService.GetAsync(id);
        if (item == null)
            throw new ItemNotFoundException();

        var items = await _itemStorageService.GetConnectionsByParentAsync(item.Id);
        var fullPath = await MakeFullPathAsync(item.Id);
        var folder = new Folder 
        {
            Name = item.Name, 
            Path = await ParsePath(fullPath),
            FullPath = string.Join(Path.DirectorySeparatorChar, fullPath),
            Guid = item.Id,
            Items = await PrepareItemsAsync(items.Select(i => i.ChildId).ToList()),
            CreationTime = item.CreationTime,
            CreatorName = "testName",
        };
        return folder;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(string? id)
    {
        if (id == null)
            return BadRequest();

        try
        {
            return new JsonResult(await GetFolderInfoAsync(id));
        }
        catch (ItemNotFoundException)
        {
            try {
                return await _fileSystemService.GetFileAsync(id);
            }
            catch (PracticeWeb.Exceptions.FileNotFoundException)
            {
                return NotFound();
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> UploadFileAsync(string? path, IFormFile uploadedFile)
    {
        if (uploadedFile == null)
            return BadRequest();
        
        try
        {
            path = PreparePath(path);
        }
        catch (NullReferenceException)
        {
            return BadRequest();
        }

        try
        {
            await _fileSystemService.CreateFileAsync(path, uploadedFile);
        }
        catch (FolderNotFoundException)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateFolderAsync(string? parentId, string? name)
    {
        if (parentId == null || await _itemStorageService.GetAsync(parentId) == null || name == null)
            return BadRequest();

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
        await _fileSystemService.CreateFolderAsync(path, pathItems.Last());

        return Ok();
    }

    [HttpPatch]
    public async Task<IActionResult> RenameAsync(string? id, string? name)
    {
        if (id == null || name == null)
            return BadRequest();

        await _fileSystemService.RenameAsync(id, name);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(string? path)
    {
        try
        {
            path = PreparePath(path);
        }
        catch (NullReferenceException)
        {
            return BadRequest();
        }

        try
        {
            await _fileSystemService.RemoveFolder(path);
        }
        catch (FolderNotFoundException)
        {
            try
            {
                await _fileSystemService.RemoveFileAsync(path);
            }
            catch (PracticeWeb.Exceptions.FileNotFoundException)
            {
                return NotFound();
            }
        }

        return Ok();
    }
}

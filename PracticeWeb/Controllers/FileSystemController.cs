using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Exceptions;
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

    [HttpGet("group")]
    public async Task<IActionResult> GetByGroup(string? group)
    {
        if (group == null)
            return BadRequest();

        var groupInfo = await _groupStorageService.GetByGroupNameAsync(group);
        if (groupInfo == null)
            return BadRequest();

        var item = await _itemStorageService.GetAsync(groupInfo.Id);
        if (item == null)
            return BadRequest();

        var folder = new Folder 
        {
            Name = item.Name, 
            Path = item.Name, 
            Guid = item.Id,
            Items = new List<FolderItem>(),
            CreationTime = item.CreationTime,
            CreatorName = "testName",
        };
        return new JsonResult(folder);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(string? path)
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
            return new JsonResult(await _fileSystemService.GetFolderInfoAsync(path));
        }
        catch (FolderNotFoundException)
        {
            try {
                return await _fileSystemService.GetFileAsync(path);
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
    public async Task<IActionResult> CreateFolderAsync(string? path, string name)
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
            await _fileSystemService.CreateFolderAsync(path, name);
        }
        catch (FolderNotFoundException)
        {
            return NotFound();
        }

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

using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;
using PracticeWeb.Services.FileSystemServices;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class FileSystemController : ControllerBase
{
    private IFileSystemService _fileSystemService;

    public FileSystemController(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;
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

    [HttpGet]
    public async Task<IActionResult> GetAsync(string? id)
    {
        if (id == null)
            return BadRequest();

        try
        {
            return new JsonResult(await _fileSystemService.GetFolderInfoAsync(id));
        }
        catch (ItemTypeException)
        {
            try {
                return await _fileSystemService.GetFileAsync(id);
            }
            catch (ItemNotFoundException)
            {
                return NotFound();
            }
            catch (ItemTypeException)
            {
                return BadRequest();
            }
        }
        catch (ItemNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("file")]
    public async Task<IActionResult> UploadFileAsync([FromForm] string? parentId, [FromForm] IFormFile? uploadedFile)
    {
        if (parentId == null || uploadedFile == null)
            return BadRequest();
        
        try
        {
            var item = await _fileSystemService.CreateFileAsync(parentId, uploadedFile);
            return new JsonResult(item);
        }
        catch (Exception ex)
        {
            if (ex is FolderNotFoundException || ex is ItemNotFoundException)
                return NotFound();
            else if (ex is ItemTypeException)
                return BadRequest();
            throw;
        }

    }

    [HttpPost]
    public async Task<IActionResult> CreateFolderAsync(
        [FromForm] string? parentId, 
        [FromForm] string? name, 
        [FromForm] string? type, 
        [FromForm] Dictionary<string, string>? parameters)
    {
        if (parentId == null || name == null || type == null)
            return BadRequest();

        try
        {
            var item = await _fileSystemService.CreateFolderAsync(parentId, name, type, parameters);
            return new JsonResult(item);
        }
        catch (Exception ex)
        {
            if (ex is FolderNotFoundException || ex is ItemNotFoundException)
                return NotFound();
            else if (ex is ItemTypeException)
                return BadRequest();
            else if (ex is InvalidPathException)
                return BadRequest();
            else if (ex is InvalidGroupNameException)
                return BadRequest();
            else if (ex is InvalidSubjectNameException)
                return BadRequest();
            throw;
        }
    }

    [HttpPatch]
    public async Task<IActionResult> RenameAsync(string? id, string? newName)
    {
        if (id == null || newName == null)
            return BadRequest();

        try
        {
            await _fileSystemService.RenameAsync(id, newName);
        }
        catch (ItemNotFoundException)
        {
            return NotFound();
        }
        catch (ItemTypeException)
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(string? id)
    {
        if (id == null)
            return BadRequest();

        try
        {
            await _fileSystemService.RemoveAsync(id);
        }
        catch (Exception ex)
        {
            if (ex is FolderNotFoundException || ex is ItemNotFoundException)
                return NotFound();
            throw;
        }

        return Ok();
    }
}

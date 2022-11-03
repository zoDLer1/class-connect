using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Exceptions;
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

    [HttpDelete]
    public async Task<IActionResult> DeleteFileAsync(string? path)
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

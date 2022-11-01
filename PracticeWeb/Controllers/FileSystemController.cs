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

    [HttpGet]
    public async Task<IActionResult> GetFolderAsync(string? path)
    {
        if (path == null)
            return BadRequest();
        
        if (path.StartsWith("/")) 
            path = path.TrimStart('/');
        if (path.EndsWith("/")) 
            path = path.TrimEnd('/');

        try
        {
            return new JsonResult(await _fileSystemService.GetFolderInfo(path));
        }
        catch (FolderNotFoundException)
        {
            try {
                return await _fileSystemService.GetFile(path);
            }
            catch (PracticeWeb.Exceptions.FileNotFoundException)
            {
                return NotFound();
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(string? path, IFormFile uploadedFile)
    {
        if (path == null || uploadedFile == null)
            return BadRequest();
        
        if (path.StartsWith("/")) 
            path = path.TrimStart('/');
        if (path.EndsWith("/")) 
            path = path.TrimEnd('/');

        try
        {
            await _fileSystemService.CreateFile(path, uploadedFile);
        }
        catch (FolderNotFoundException)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateFolder(string? path, string name)
    {
        if (path == null)
            return BadRequest();
        
        if (path.StartsWith("/")) 
            path = path.TrimStart('/');
        if (path.EndsWith("/")) 
            path = path.TrimEnd('/');

        try
        {
            await _fileSystemService.CreateFolder(path, name);
        }
        catch (FolderNotFoundException)
        {
            return NotFound();
        }

        return Ok();
    }
}

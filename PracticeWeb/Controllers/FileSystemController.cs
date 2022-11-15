using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;
using PracticeWeb.Services.FileSystemServices;
using PracticeWeb.Services.UserServices;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class FileSystemController : ControllerBase
{
    private IFileSystemService _fileSystemService;
    private IUserService _userService;
    private Context _context;

    public FileSystemController(IFileSystemService fileSystemService, IUserService userService, Context context)
    {
        _fileSystemService = fileSystemService;
        _userService = userService;
        _context = context;
    }

    private void CreateDirectory(string path) 
    {
        Directory.CreateDirectory(path);
    }

    private async Task<User> GetUserAsync()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            throw new UserNotFoundException();
        
        var user = await _userService.GetByEmailAsync(email);
        if (user == null)
            throw new UserNotFoundException();

        return user;
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
            return BadRequest(new { errrorText = "Недостаточно параметров" });

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
                return NotFound(new { errrorText = "Файл не найден" });
            }
            catch (ItemTypeException)
            {
                return BadRequest(new { errrorText = "Неверный тип файла" });
            }
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errrorText = "Объект не найден" });
        }
        catch (FolderNotFoundException)
        {
            return NotFound(new { errrorText = "Папка не найдена" });
        }
    }

    [Authorize(Roles = "Student")]
    [HttpPost("work")]
    public async Task<IActionResult> UploadWork(
        [FromForm] string? parentId, 
        [FromForm] IFormFile? uploadedFile)
    {
        if (parentId == null || uploadedFile == null)
            return BadRequest(new { errrorText = "Недостаточно параметров" });

        try
        {
            var user = await GetUserAsync();
            var fullUserName = string.Join(' ', new { user.FirstName, user.LastName, user.Patronymic });
            var folder = await _fileSystemService.CreateFolderAsync(parentId, fullUserName, "Work", user.Id, null);
            var file = await _fileSystemService.CreateFileAsync(folder.Guid, uploadedFile, user.Id);
            return new JsonResult(file);
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { errrorText = "Студент не найден" });
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errrorText = "Объект не найден" });
        }
        catch (FolderNotFoundException)
        {
            return NotFound(new { errrorText = "Папка не найдена" });
        }
        catch (InvalidPathException)
        {
            return BadRequest(new { errrorText = "Передан неправильный родитель" });
        }
        catch (InvalidGroupNameException)
        {
            return BadRequest(new { errrorText = "Неправильное название группы" });
        }
        catch (InvalidSubjectNameException)
        {
            return BadRequest(new { errrorText = "Неправильное название предмета" });
        }
        catch (KeyNotFoundException)
        {
            return BadRequest(new { errrorText = "Невалидное значение параметра Type" });
        }
    }

    [HttpPost("file")]
    public async Task<IActionResult> UploadFileAsync(
        [FromForm] string? parentId, 
        [FromForm] IFormFile? uploadedFile)
    {
        if (parentId == null || uploadedFile == null)
            return BadRequest(new { errrorText = "Недостаточно параметров" });

        try
        {
            var user = await GetUserAsync();
            var item = await _fileSystemService.CreateFileAsync(parentId, uploadedFile, user.Id);
            return new JsonResult(item);
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { errrorText = "Пользователь не найден" });
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errrorText = "Объект не найден" });
        }
        catch (FolderNotFoundException)
        {
            return NotFound(new { errrorText = "Папка не найдена" });
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
            return BadRequest(new { errrorText = "Недостаточно параметров" });

        try
        {
            var user = await GetUserAsync();
            var item = await _fileSystemService.CreateFolderAsync(parentId, name, type, user.Id, parameters);
            return new JsonResult(item);
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { errrorText = "Пользователь не найден" });
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errrorText = "Объект не найден" });
        }
        catch (FolderNotFoundException)
        {
            return NotFound(new { errrorText = "Папка не найдена" });
        }
        catch (InvalidPathException)
        {
            return BadRequest(new { errrorText = "Передан неправильный родитель" });
        }
        catch (InvalidGroupNameException)
        {
            return BadRequest(new { errrorText = "Неправильное название группы" });
        }
        catch (InvalidSubjectNameException)
        {
            return BadRequest(new { errrorText = "Неправильное название предмета" });
        }
        catch (KeyNotFoundException)
        {
            return BadRequest(new { errrorText = "Невалидное значение параметра Type" });
        }
    }

    [HttpPatch]
    public async Task<IActionResult> RenameAsync(string? id, string? newName)
    {
        if (id == null || newName == null)
            return BadRequest(new { errrorText = "Недостаточно параметров" });

        try
        {
            await _fileSystemService.RenameAsync(id, newName);
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errrorText = "Объект не найден" });
        }
        catch (InvalidGroupNameException)
        {
            return BadRequest(new { errrorText = "Неправильное название группы" });
        }
        catch (InvalidSubjectNameException)
        {
            return BadRequest(new { errrorText = "Неправильное название предмета" });
        }

        return Ok();
    }

    [HttpPatch("type")]
    public async Task<IActionResult> UpdateTypeAsync(string? id, string newType)
    {
        if (id == null || newType == null)
            return BadRequest(new { errrorText = "Недостаточно параметров" });
        
        try
        {
            await _fileSystemService.UpdateTypeAsync(id, newType);
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errrorText = "Объект не найден" });
        }
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(string? id)
    {
        if (id == null)
            return BadRequest(new { errrorText = "Недостаточно параметров" });

        try
        {
            await _fileSystemService.RemoveAsync(id);
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errrorText = "Объект не найден" });
        }
        catch (FolderNotFoundException)
        {
            return NotFound(new { errrorText = "Папка не найдена" });
        }

        return Ok();
    }
}

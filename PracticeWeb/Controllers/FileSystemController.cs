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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAsync(string? id)
    {
        if (id == null)
            return BadRequest(new { errorText = "Недостаточно параметров" });

        try
        {
            var user = await GetUserAsync();
            return new JsonResult(await _fileSystemService.GetObjectAsync(id, user));
        }
        catch (ItemTypeException)
        {
            try {
                var user = await GetUserAsync();
                return await _fileSystemService.GetFileAsync(id, user);
            }
            catch (ItemNotFoundException)
            {
                return NotFound(new { errorText = "Файл не найден" });
            }
            catch (ItemTypeException)
            {
                return BadRequest(new { errorText = "Неверный тип файла" });
            }
            catch (UserNotFoundException)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errorText = "Объект не найден" });
        }
        catch (FolderNotFoundException)
        {
            return NotFound(new { errorText = "Папка не найдена" });
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { errorText = "Пользователь не найден" });
        }
        catch (AccessDeniedException)
        {
            return Forbid();
        }
    }

    [Authorize(Roles = "Student")]
    [HttpPost("work")]
    public async Task<IActionResult> UploadWork(
        [FromForm] string? parentId, 
        [FromForm] IFormFile? uploadedFile)
    {
        if (parentId == null || uploadedFile == null)
            return BadRequest(new { errorText = "Недостаточно параметров" });

        try
        {
            var user = await GetUserAsync();
            var fullUserName = string.Join(' ', new[] { user.FirstName, user.LastName, user.Patronymic });
            var work = await _fileSystemService.CreateWorkAsync(parentId, fullUserName, "Work", uploadedFile, user);
            return new JsonResult(work);
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { errorText = "Студент не найден" });
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errorText = "Объект не найден" });
        }
        catch (FolderNotFoundException)
        {
            return NotFound(new { errorText = "Папка не найдена" });
        }
        catch (InvalidPathException)
        {
            return BadRequest(new { errorText = "Передан неправильный родитель" });
        }
        catch (InvalidGroupNameException)
        {
            return BadRequest(new { errorText = "Неправильное название группы" });
        }
        catch (InvalidSubjectNameException)
        {
            return BadRequest(new { errorText = "Неправильное название предмета" });
        }
        catch (KeyNotFoundException)
        {
            return BadRequest(new { errorText = "Невалидное значение параметра Type" });
        }
        catch (AccessDeniedException)
        {
            return Forbid();
        }
    }

    [Authorize(Roles = "Teacher,Admin")]
    [HttpPost("file")]
    public async Task<IActionResult> UploadFileAsync(
        [FromForm] string? parentId, 
        [FromForm] IFormFile uploadedFile)
    {
        if (parentId == null || uploadedFile == null)
            return BadRequest(new { errorText = "Недостаточно параметров" });

        try
        {
            var user = await GetUserAsync();
            var item = await _fileSystemService.CreateFileAsync(parentId, uploadedFile, user);
            return new JsonResult(item);
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { errorText = "Пользователь не найден" });
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errorText = "Объект не найден" });
        }
        catch (FolderNotFoundException)
        {
            return NotFound(new { errorText = "Папка не найдена" });
        }
        catch (AccessDeniedException)
        {
            return Forbid();
        }
    }

    [Authorize(Roles = "Teacher,Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateFolderAsync(
        [FromForm] string? parentId, 
        [FromForm] string? name, 
        [FromForm] string? type, 
        [FromForm] Dictionary<string, string>? parameters)
    {
        if (parentId == null || name == null || type == null)
            return BadRequest(new { errorText = "Недостаточно параметров" });

        try
        {
            var user = await GetUserAsync();
            var item = await _fileSystemService.CreateFolderAsync(parentId, name, type, user, parameters);
            return new JsonResult(item);
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { errorText = "Пользователь не найден" });
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errorText = "Объект не найден" });
        }
        catch (FolderNotFoundException)
        {
            return NotFound(new { errorText = "Папка не найдена" });
        }
        catch (InvalidPathException)
        {
            return BadRequest(new { errorText = "Передан неправильный родитель" });
        }
        catch (InvalidGroupNameException)
        {
            return BadRequest(new { errorText = "Неправильное название группы" });
        }
        catch (InvalidSubjectNameException)
        {
            return BadRequest(new { errorText = "Неправильное название предмета" });
        }
        catch (KeyNotFoundException)
        {
            return BadRequest(new { errorText = "Невалидное значение параметра Type" });
        }
        catch (NullReferenceException)
        {
            return BadRequest(new { errorText = "Недостаточно параметров" });
        }
    }

    [Authorize(Roles = "Teacher,Admin")]
    [HttpPatch]
    public async Task<IActionResult> RenameAsync(string? id, string? newName)
    {
        if (id == null || newName == null)
            return BadRequest(new { errorText = "Недостаточно параметров" });

        try
        {
            var user = await GetUserAsync();
            await _fileSystemService.RenameAsync(id, newName, user);
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { errorText = "Пользователь не найден" });
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errorText = "Объект не найден" });
        }
        catch (InvalidGroupNameException)
        {
            return BadRequest(new { errorText = "Неправильное название группы" });
        }
        catch (InvalidSubjectNameException)
        {
            return BadRequest(new { errorText = "Неправильное название предмета" });
        }
        catch (AccessDeniedException)
        {
            return Forbid();
        }

        return Ok();
    }

    [Authorize(Roles = "Teacher,Admin")]
    [HttpPatch("type")]
    public async Task<IActionResult> UpdateTypeAsync(string? id, string? newType)
    {
        if (id == null || newType == null)
            return BadRequest(new { errorText = "Недостаточно параметров" });
        
        try
        {
            var user = await GetUserAsync();
            await _fileSystemService.UpdateTypeAsync(id, newType, user);
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { errorText = "Пользователь не найден" });
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errorText = "Объект не найден" });
        }
        catch (AccessDeniedException)
        {
            return Forbid();
        }
        
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(string? id)
    {
        if (id == null)
            return BadRequest(new { errorText = "Недостаточно параметров" });

        try
        {
            await _fileSystemService.RemoveAsync(id);
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errorText = "Объект не найден" });
        }
        catch (FolderNotFoundException)
        {
            return NotFound(new { errorText = "Папка не найдена" });
        }

        return Ok();
    }
}

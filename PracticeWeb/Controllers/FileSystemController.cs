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
            var resultObject = await _fileSystemService.GetObjectAsync(id, user);
            if (resultObject is IActionResult result)
                return result;
            return new JsonResult(resultObject);
        }
        catch (ItemTypeException)
        {
            return BadRequest(new { errorText = "Неверный тип объекта" });
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

    [Authorize]
    [HttpPost("{workId}")]
    public async Task<IActionResult> MarkWork(string? workId, [FromForm] int? mark)
    {
        if (workId == null)
            return BadRequest(new { errorText = "Недостаточно параметров" });

        try
        {
            var user = await GetUserAsync();
            object result;
            if (user.RoleId == UserRole.Student)
            {
                result = await _fileSystemService.SubmitWork(workId, user);
            }
            else
            {
                if (mark == null)
                    return BadRequest(new { errorText = "Недостаточно параметров" });

                if (mark < 2 || mark > 5)
                    return BadRequest(new { errorText = "Некорректное значение" });
                result = await _fileSystemService.MarkWork(workId, (int)mark, user);
            }
            return new JsonResult(result);
        }
        catch (AccessDeniedException)
        {
            return Forbid();
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errorText = "Объект не найден" });
        }
        catch (ItemTypeException)
        {
            return BadRequest(new { errorText = "Передан объект с неправильным типом" });
        }
    }

    [Authorize]
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
            object result;
            if (user.RoleId == UserRole.Student)
            {
                var fullUserName = string.Join(' ', new[] { user.Name, user.Surname, user.Patronymic });
                result = await _fileSystemService.CreateWorkAsync(parentId, fullUserName, "Work", uploadedFile, user);
            }
            else
            {
                result = await _fileSystemService.CreateFileAsync(parentId, uploadedFile, user);
            }
            return new JsonResult(result);
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { errorText = "Пользователь не найден" });
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errorText = "Объект не найден" });
        }
        catch (ItemTypeException)
        {
            return BadRequest(new { errorText = "Передан объект с неправильным типом" });
        }
        catch (FolderNotFoundException)
        {
            return NotFound(new { errorText = "Папка не найдена" });
        }
        catch (InvalidPathException)
        {
            return BadRequest(new { errorText = "Передан неправильный родитель" });
        }
        catch (AccessDeniedException)
        {
            return Forbid();
        }
    }

    [Authorize(Roles = "Teacher,Administrator")]
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
        catch (InvalidUserRoleException)
        {
            return BadRequest(new { errorText = "Неправильная роль пользователя" });
        }
        catch (KeyNotFoundException)
        {
            return BadRequest(new { errorText = "Невалидное значение параметра Type" });
        }
        catch (NullReferenceException)
        {
            return BadRequest(new { errorText = "Недостаточно параметров" });
        }
        catch (AccessDeniedException)
        {
            return Forbid();
        }
    }

    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> RenameAsync(string? id, string? newName)
    {
        Console.WriteLine($"{id} {newName}");
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
        catch (InvalidPathException)
        {
            return BadRequest(new { errorText = "Передан неправильный айди" });
        }
        catch (AccessDeniedException)
        {
            return Forbid();
        }

        return Ok();
    }

    [Authorize(Roles = "Teacher,Administrator")]
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
        catch (ItemTypeException)
        {
            return BadRequest(new { errorText = "Передан неправильный тип" });
        }
        catch (InvalidPathException)
        {
            return BadRequest(new { errorText = "Передан неправильный айди" });
        }
        catch (AccessDeniedException)
        {
            return Forbid();
        }

        return Ok();
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(string? id)
    {
        if (id == null)
            return BadRequest(new { errorText = "Недостаточно параметров" });

        try
        {
            var user = await GetUserAsync();
            await _fileSystemService.RemoveAsync(id, user);
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
            return BadRequest(new { errorText = "Передан неправильный айди" });
        }
        catch (AccessDeniedException)
        {
            return Forbid();
        }

        return Ok();
    }
}

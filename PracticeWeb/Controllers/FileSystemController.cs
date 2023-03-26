using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticeWeb.Controllers.Models;
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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] ItemModel model)
    {
        try
        {
            var user = await GetUserAsync();
            var resultObject = await _fileSystemService.GetObjectAsync(model.Id, user);
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
            return BadRequest(new { errorText = "Пользователь не найден" });
        }
        catch (AccessDeniedException)
        {
            return Forbid();
        }
    }

    [Authorize(Roles = "Teacher,Administrator")]
    [HttpPost]
    public async Task<IActionResult> CreateFolderAsync([FromBody] FolderModel model)
    {
        var parameters = new Dictionary<string, object>();
        if (model.TeacherId != null)
            parameters["TeacherId"] = model.TeacherId;

        if (model.Description != null)
            parameters["Description"] = model.Description;

        if (model.Until != null)
            parameters["Until"] = model.Until;

        try
        {
            var user = await GetUserAsync();
            var item = await _fileSystemService.CreateFolderAsync(
                model.Id,
                model.Name,
                (Type)Enum.Parse(typeof(Type), model.Type),
                user,
                parameters
            );
            return new JsonResult(item);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new { errorText = "Пользователь не найден" });
        }
        catch (TeacherNotFoundException)
        {
            return BadRequest(new { Errors = new { TeacherId = new List<string> { "Преподаватель не найден" } } });
        }
        catch (InvalidDateException)
        {
            return BadRequest(new { Errors = new { Until = new List<string> { "Некорректная дата" } } });
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
            return BadRequest(new { Errors = new { Name = new List<string> { "Неправильное название группы" } } });
        }
        catch (InvalidSubjectNameException)
        {
            return BadRequest(new { Errors = new { Name = new List<string> { "Неправильное название предмета" } } });
        }
        catch (InvalidUserRoleException)
        {
            return BadRequest(new { errorText = "Неправильная роль пользователя" });
        }
        catch (KeyNotFoundException)
        {
            return BadRequest(new { Errors = new { Type = new List<string> { "Некорректное значение параметра Type" } } });
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
    [HttpPost("file")]
    public async Task<IActionResult> UploadFileAsync([FromForm] FileModel model)
    {
        try
        {
            var user = await GetUserAsync();
            object result;
            if (user.RoleId == UserRole.Student)
            {
                var fullUserName = string.Join(' ', new[] { user.Name, user.Surname, user.Patronymic });
                result = await _fileSystemService.CreateWorkAsync(model.Id, fullUserName, model.UploadedFile, user);
            }
            else
            {
                result = await _fileSystemService.CreateFileAsync(model.Id, model.UploadedFile, user);
            }
            return new JsonResult(result);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new { errorText = "Пользователь не найден" });
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

    [Authorize]
    [HttpPost("mark")]
    public async Task<IActionResult> MarkWork([FromBody] WorkModel model)
    {
        try
        {
            var user = await GetUserAsync();
            object result;
            if (user.RoleId == UserRole.Student)
            {
                result = await _fileSystemService.SubmitWork(model.Id, user);
            }
            else
            {
                if (model.Mark != null && model.Mark < 2 || model.Mark > 5)
                    return BadRequest(new { Errors = new { Mark = new List<string> { "Некорректное значение оценки" } } });

                result = await _fileSystemService.MarkWork(model.Id, model.Mark, user);
            }
            return new JsonResult(result);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new { errorText = "Пользователь не найден" });
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
    [HttpPatch]
    public async Task<IActionResult> RenameAsync([FromQuery] NewNameItemModel model)
    {
        try
        {
            var user = await GetUserAsync();
            await _fileSystemService.RenameAsync(model.Id, model.Name, user);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new { errorText = "Пользователь не найден" });
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errorText = "Объект не найден" });
        }
        catch (InvalidGroupNameException)
        {
            return BadRequest(new { Errors = new { Name = new List<string> { "Неправильное название группы" } } });
        }
        catch (InvalidSubjectNameException)
        {
            return BadRequest(new { Errors = new { Name = new List<string> { "Неправильное название предмета" } } });
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
    public async Task<IActionResult> UpdateTypeAsync([FromQuery] NewTypeItemModel model)
    {
        try
        {
            var user = await GetUserAsync();
            await _fileSystemService.UpdateTypeAsync(model.Id, (Type)Enum.Parse(typeof(Type), model.Type), user);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new { errorText = "Пользователь не найден" });
        }
        catch (ItemNotFoundException)
        {
            return NotFound(new { errorText = "Объект не найден" });
        }
        catch (ItemTypeException)
        {
            return BadRequest(new { Errors = new { Type = new List<string> { "Некорректное значение параметра Type" } } });
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
    public async Task<IActionResult> DeleteAsync([FromQuery] ItemModel model)
    {
        try
        {
            var user = await GetUserAsync();
            await _fileSystemService.RemoveAsync(model.Id, user);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new { errorText = "Пользователь не найден" });
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

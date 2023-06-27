using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClassConnect.Controllers.Models;
using ClassConnect.Exceptions;
using ClassConnect.Models;
using ClassConnect.Services.FileSystemServices;
using ClassConnect.Services.UserServices;

namespace ClassConnect.Controllers;

[ApiController]
[Route("[controller]")]
public class FileSystemController : Controller
{
    private IFileSystemService _fileSystemService;
    private Context _context;

    public FileSystemController(
        IFileSystemService fileSystemService,
        IUserService userService,
        Context context
    )
        : base(userService)
    {
        _fileSystemService = fileSystemService;
        _context = context;
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
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
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
                (Item)Enum.Parse(typeof(Item), model.Type),
                user,
                parameters
            );
            return new JsonResult(item);
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
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
                var fullUserName = string.Join(
                    ' ',
                    new[] { user.Name, user.Surname, user.Patronymic }
                );
                result = await _fileSystemService.CreateWorkAsync(
                    model.Id,
                    fullUserName,
                    model.UploadedFile,
                    user
                );
            }
            else
            {
                result = await _fileSystemService.CreateFileAsync(
                    model.Id,
                    model.UploadedFile,
                    user
                );
            }
            return new JsonResult(result);
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
        }
    }

    [Authorize]
    [HttpPost("mark")]
    public async Task<IActionResult> MarkWorkAsync([FromBody] WorkModel model)
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
                    return BadRequest(
                        new
                        {
                            Errors = new
                            {
                                Mark = new List<string> { "Некорректное значение оценки" }
                            }
                        }
                    );

                result = await _fileSystemService.MarkWork(model.Id, model.Mark, user);
            }
            return new JsonResult(result);
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
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
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
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
            await _fileSystemService.UpdateTypeAsync(
                model.Id,
                (Item)Enum.Parse(typeof(Item), model.Type),
                user
            );
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
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
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
        }

        return Ok();
    }
}

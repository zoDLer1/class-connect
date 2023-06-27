using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassConnect.Controllers.Models;
using ClassConnect.Exceptions;
using ClassConnect.Models;
using ClassConnect.Services;
using ClassConnect.Services.UserServices;

namespace ClassConnect.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupController : Controller
{
    private Context _context;
    private CommonQueries<string, Group> _commonGroupQueries;
    private IUserService _userService;

    public GroupController(Context context, IUserService userService)
        : base(userService)
    {
        _context = context;
        _commonGroupQueries = new CommonQueries<string, Group>(_context);
        _userService = userService;
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public async Task<IActionResult> GetGroupsAsync()
    {
        try
        {
            await GetUserAsync();
            var groups = _context.Items
                .Where(i => i.TypeId == Item.Group)
                .Select(g => new { Id = g.Id, Name = g.Name });
            return new JsonResult(groups);
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
        }
    }

    [Authorize(Roles = "Student")]
    [HttpGet("enter")]
    public async Task<IActionResult> EnterGroupAsync([FromQuery] ItemModel model)
    {
        try
        {
            await GetUserAsync();
            var group = await _commonGroupQueries.GetAsync(model.Id, _context.Groups);
            if (group == null)
                return NotFound(new { errorText = "Группа не найдена" });

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                return Unauthorized(new { errorText = "Пользователь не найден" });

            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return BadRequest(new { errorText = "Студент не найден" });

            if (await _context.Accesses.FirstOrDefaultAsync(s => s.UserId == user.Id) != null)
                return BadRequest(new { errorText = "Вы уже добавлены в группу" });

            var studentAccess = new Access
            {
                Permission = Permission.Read,
                ItemId = group.Id,
                UserId = user.Id
            };
            await _context.Accesses.AddAsync(studentAccess);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
        }

        return Ok();
    }

    [Authorize(Roles = "Teacher,Administrator")]
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveStudentAsync([FromQuery] StudentModel model)
    {
        try
        {
            await GetUserAsync();
            var group = await _commonGroupQueries.GetAsync(model.Id, _context.Groups);
            if (group == null)
                return NotFound(new { errorText = "Группа не найдена" });

            var student = await _userService.GetAsync((int)model.StudentId);
            if (student == null)
                return NotFound(new { errorText = "Студент не найден" });

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                return Unauthorized(new { errorText = "Пользователь не найден" });

            var teacher = await _userService.GetByEmailAsync(email);
            if (teacher == null)
                return BadRequest(new { errorText = "Преподаватель не найден" });

            if (group.TeacherId != teacher.Id && teacher.RoleId == UserRole.Teacher)
                return BadRequest(
                    new { errorText = "Пользователь не является преподавателем этой группы" }
                );

            var studentAccess = await _context.Accesses.FirstOrDefaultAsync(
                s => s.UserId == student.Id && s.ItemId == group.Id
            );
            if (studentAccess == null)
                return BadRequest(new { errorText = "Студента нет в группе" });

            _context.Accesses.Remove(studentAccess);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
        }

        return Ok();
    }
}

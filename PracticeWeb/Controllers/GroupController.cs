using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeWeb.Models;
using PracticeWeb.Services;
using PracticeWeb.Services.UserServices;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupController : ControllerBase
{
    private Context _context;
    private CommonQueries<string, Group> _commonGroupQueries;
    private IUserService _userService;

    public GroupController(Context context, IUserService userService)
    {
        _context = context;
        _commonGroupQueries = new CommonQueries<string, Group>(_context);
        _userService = userService;
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("teachers")]
    public IActionResult GetGroups()
    {
        var groups = _context.Groups
            .Select(g => new 
                {
                    Id = g.Id,
                    Name = g.Name
                }
            );
        return new JsonResult(groups);   
    }

    [Authorize(Roles = "Student")]
    [HttpPost("enter")]
    public async Task<IActionResult> EnterGroup([FromForm] string? groupId)
    {
        if (groupId == null)
            return BadRequest(new { errorText = "Недостаточно параметров" });

        var group = await _commonGroupQueries.GetAsync(groupId, _context.Groups);
        if (group == null)
            return NotFound(new { errorText = "Группа не найдена" });
        
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return Unauthorized(new { errorText = "Пользователь не найден" });
        
        var user = await _userService.GetByEmailAsync(email);
        if (user == null)
            return BadRequest(new { errorText = "Студент не найден" });

        if (await _context.GroupStudents.FirstOrDefaultAsync(s => s.StudentId == user.Id && s.GroupId == group.Id) != null)
            return BadRequest(new { errorText = "Данный пользователь уже добавлен в группу" });

        var student = new GroupStudent{
            GroupId = group.Id,
            StudentId = user.Id
        };
        await _context.GroupStudents.AddAsync(student);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [Authorize(Roles = "Teacher,Administrator")]
    [HttpPost("remove")]
    public async Task<IActionResult> RemoveStudent([FromForm] string? groupId, [FromForm] int? studentId)
    {
        if (groupId == null || studentId == null)
            return BadRequest(new { errorText = "Недостаточно параметров" });

        var group = await _commonGroupQueries.GetAsync(groupId, _context.Groups);
        if (group == null)
            return NotFound(new { errorText = "Группа не найдена" });

        var student = await _userService.GetAsync((int) studentId);
        if (student == null)
            return NotFound(new { errorText = "Студент не найден" });
        
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
            return Unauthorized(new { errorText = "Пользователь не найден" });
        
        var teacher = await _userService.GetByEmailAsync(email);
        if (teacher == null)
            return BadRequest(new { errorText = "Преподаватель не найден" });

        if (group.TeacherId != teacher.Id && teacher.Role.Name == "Teacher")
            return BadRequest(new { errorText = "Пользователь не является преподавателем этой группы" });
        
        var groupStudent = await _context.GroupStudents.FirstOrDefaultAsync(s => s.StudentId == student.Id && s.GroupId == group.Id);
        if (groupStudent == null)
            return BadRequest(new { errorText = "Студента нет в группе" });

        _context.GroupStudents.Remove(groupStudent);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
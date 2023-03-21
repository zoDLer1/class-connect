using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PracticeWeb.Controllers.Models;
using PracticeWeb.Exceptions;
using PracticeWeb.Models;
using PracticeWeb.Services;
using PracticeWeb.Services.AuthenticationServices;
using PracticeWeb.Services.FileSystemServices;
using PracticeWeb.Services.UserServices;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private Context _context;
    private CommonQueries<string, Group> _commonGroupQueries;
    private IAuthenticationService _authenticationService;
    private IFileSystemService _fileSystemService;
    private IUserService _userService;

    public UserController(
        Context context,
        IAuthenticationService authenticationService,
        IUserService userService,
        IFileSystemService fileSystemService)
    {
        _context = context;
        _commonGroupQueries = new CommonQueries<string, Group>(_context);
        _authenticationService = authenticationService;
        _fileSystemService = fileSystemService;
        _userService = userService;
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("teachers")]
    public IActionResult GetTeachers()
    {
        var teachers = _context.Users.Include(u => u.Role)
            .Where(u => u.RoleId == UserRole.Teacher)
            .Select(u => new
                {
                    Id = u.Id,
                    FirstName = u.Name,
                    LastName = u.Surname,
                    Patronymic = u.Patronymic
                }
            );
        return new JsonResult(teachers);
    }

    private async Task<bool> IsEmailUsedAsync(string email)
    {
        return await _userService.GetByEmailAsync(email) != null;
    }

    private ClaimsIdentity GetIdentity(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
            new Claim(ClaimTypes.Surname, user.Surname),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
        };
        ClaimsIdentity identity = new ClaimsIdentity(
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
        return identity;
    }

    private object CreateToken(User user)
    {
        var identity = GetIdentity(user);
        var now = DateTime.UtcNow;
        var expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME));
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: identity.Claims,
            expires: expires,
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        return new
        {
            Token = encodedJwt,
            Created = now,
            Expires = expires
        };
    }

    private RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Created = DateTime.Now,
            Expires = DateTime.Now.AddDays(2),
        };
        return refreshToken;
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenModel? model)
    {
        if (model == null)
            return BadRequest(new { errorText = "Неверный RefreshToken" });

        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.RefreshToken)
            .FirstOrDefaultAsync(u => u.RefreshTokenId == model.Token);

        if (user == null || user.RefreshToken == null)
            return Unauthorized(new { errorText = "Неверный RefreshToken" });
        else if (user.RefreshToken.Expires < DateTime.Now)
            return Unauthorized(new { errorText = "RefreshToken истёк" });

        var jwt = CreateToken(user);
        var newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        await _userService.UpdateAsync(user);

        return new JsonResult(
            new {
                accessToken = jwt,
                refreshToken = new {
                    token = newRefreshToken.Token,
                    created = newRefreshToken.Created,
                    expires = newRefreshToken.Expires
                }
            }
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        try
        {
            var user = await _authenticationService.LoginAsync(model.Email, model.Password);
            var jwt = CreateToken(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            await _userService.UpdateAsync(user);

            var folder = _fileSystemService.RootGuid;
            if (user.RoleId == UserRole.Student)
            {
                var group = await _context.Accesses.FirstOrDefaultAsync(s => s.UserId == user.Id);
                folder = group?.ItemId;
            }

            return new JsonResult(
                new {
                    accessToken = jwt,
                    refreshToken = new {
                        token = refreshToken.Token,
                        created = refreshToken.Created,
                        expires = refreshToken.Expires
                    },
                    user = new {
                        name = user.Name,
                        surname = user.Surname,
                        role = user.Role.Name,
                        folder = folder,
                    }
                }
            );
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { Errors = new { Email = new List<string> { "Пользователь не найден" } } });
        }
        catch (InvalidPasswordException)
        {
            return BadRequest(new { Errors = new { Email = new List<string> { "Неправильный пароль" } } });
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupModel model)
    {
        if (model.Patronymic != string.Empty && model.Patronymic.Length < 5)
            return BadRequest(new { Errors = new { Patronymic = new List<string> { "Используйте не менее 5 символов" } } });

        if (await IsEmailUsedAsync(model.Email))
            return BadRequest(new { Errors = new { Email = new List<string> { "Данная почта уже используется" } } });

        await _authenticationService.RegisterAsync(model.Name, model.Surname, model.Patronymic, model.Email, model.Password, UserRole.Student);
        return Ok();
    }
}

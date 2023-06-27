using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ClassConnect.Controllers.Models;
using ClassConnect.Exceptions;
using ClassConnect.Models;
using ClassConnect.Services;
using ClassConnect.Services.AuthenticationServices;
using ClassConnect.Services.FileSystemServices;
using ClassConnect.Services.UserServices;

namespace ClassConnect.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : Controller
{
    private Context _context;
    private CommonQueries<string, Group> _commonGroupQueries;
    private IOptions<AuthSettings> _authSettings;
    private IAuthenticationService _authenticationService;
    private IFileSystemService _fileSystemService;
    private IUserService _userService;

    public UserController(
        Context context,
        IOptions<AuthSettings> authSettings,
        IAuthenticationService authenticationService,
        IUserService userService,
        IFileSystemService fileSystemService
    )
        : base(userService)
    {
        _context = context;
        _authSettings = authSettings;
        _commonGroupQueries = new CommonQueries<string, Group>(_context);
        _authenticationService = authenticationService;
        _fileSystemService = fileSystemService;
        _userService = userService;
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id)
    {
        try
        {
            await GetUserAsync();
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new UserNotFoundException();

            return new JsonResult(
                new
                {
                    Id = user.Id,
                    FirstName = user.Name,
                    LastName = user.Surname,
                    Patronymic = user.Patronymic,
                    Role = user.Role.Name
                }
            );
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("{role}")]
    public async Task<IActionResult> GetByRoleAsync(UserRole role)
    {
        try
        {
            await GetUserAsync();
            var users = _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId == role)
                .OrderBy(u => u.Name)
                .Select(
                    u =>
                        new
                        {
                            Id = u.Id,
                            FirstName = u.Name,
                            LastName = u.Surname,
                            Patronymic = u.Patronymic,
                            Role = u.Role.Name
                        }
                );
            return new JsonResult(users);
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet()]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            await GetUserAsync();
            var users = _context.Users
                .Include(u => u.Role)
                .OrderByDescending(u => u.RoleId)
                .ThenBy(u => u.Name)
                .Select(
                    u =>
                        new
                        {
                            Id = u.Id,
                            FirstName = u.Name,
                            LastName = u.Surname,
                            Patronymic = u.Patronymic,
                            Role = u.Role.Name
                        }
                );
            return new JsonResult(users);
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] UserModel model)
    {
        try
        {
            await GetUserAsync();
            var role = (UserRole)Enum.Parse(typeof(UserRole), model.Role);
            if (role == UserRole.Student)
                return BadRequest(
                    new { Errors = new { Role = new List<string> { "Некорректная роль" } } }
                );

            return await SignupAsync(model, role);
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync([FromBody] int id)
    {
        try
        {
            await GetUserAsync();
            var user = await _userService.GetAsync(id);
            if (user == null)
                throw new UserNotFoundException();

            await _userService.DeleteAsync(user.Id);
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
        }

        return Ok();
    }

    [HttpPost("activate")]
    public async Task<IActionResult> ActivateAsync([FromBody] string link)
    {
        try
        {
            await GetUserAsync();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ActivationLink == link);
            if (user == null)
                throw new UserNotFoundException();

            if (user.IsActivated)
                return BadRequest(new { errorText = "Аккаунт уже активирован" });

            user.IsActivated = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
        }

        return Ok();
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
            ClaimsIdentity.DefaultRoleClaimType
        );
        return identity;
    }

    private object CreateToken(User user)
    {
        var identity = GetIdentity(user);
        var now = DateTime.UtcNow;
        var expires = now.Add(TimeSpan.FromMinutes(_authSettings.Value.Lifetime));
        var jwt = new JwtSecurityToken(
            issuer: _authSettings.Value.Issuer,
            audience: _authSettings.Value.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: expires,
            signingCredentials: new SigningCredentials(
                _authSettings.Value.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256
            )
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
    public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenModel? model)
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
        await _userService.UpdateAsync(user);

        return new JsonResult(new { accessToken = jwt, refreshToken = user.RefreshToken });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
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
                new
                {
                    accessToken = jwt,
                    refreshToken = new
                    {
                        token = refreshToken.Token,
                        created = refreshToken.Created,
                        expires = refreshToken.Expires
                    },
                    user = new
                    {
                        name = user.Name,
                        surname = user.Surname,
                        role = user.Role.Name,
                        folder = folder,
                    }
                }
            );
        }
        catch (Exception ex)
        {
            return ExceptionHandler.Handle(ex);
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignupAsync(
        [FromBody] SignupModel model,
        UserRole role = UserRole.Student
    )
    {
        if (model.Patronymic != string.Empty && model.Patronymic.Length < 5)
            return BadRequest(
                new
                {
                    Errors = new
                    {
                        Patronymic = new List<string> { "Используйте не менее 5 символов" }
                    }
                }
            );

        if (await IsEmailUsedAsync(model.Email))
            return BadRequest(
                new
                {
                    Errors = new { Email = new List<string> { "Данная почта уже используется" } }
                }
            );

        await _authenticationService.RegisterAsync(
            model.Name,
            model.Surname,
            model.Patronymic,
            model.Email,
            model.Password,
            role
        );
        return Ok();
    }
}

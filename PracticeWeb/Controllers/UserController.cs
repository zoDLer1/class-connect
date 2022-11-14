using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PracticeWeb.Exceptions;
using PracticeWeb.Services.AuthenticationServices;
using PracticeWeb.Services.FileSystemServices;
using PracticeWeb.Services.UserServices;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private IAuthenticationService _authenticationService;
    private IFileSystemService _fileSystemService;
    private IUserService _userService;

    public UserController(IAuthenticationService authenticationService, IUserService userService, IFileSystemService fileSystemService)
    {
        _authenticationService = authenticationService;
        _fileSystemService = fileSystemService;
        _userService = userService;
    }

    public async Task<bool> IsEmailUsedAsync(string email)
    {
        return await _userService.GetByEmailAsync(email) != null;
    }

    private async Task<ClaimsIdentity> GetIdentityAsync(string email, string password)
    {
        var user = await _authenticationService.LoginAsync(email, password);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
        };
        ClaimsIdentity identity = new ClaimsIdentity(
            claims, 
            "Token", 
            ClaimsIdentity.DefaultNameClaimType, 
            ClaimsIdentity.DefaultRoleClaimType);
        return identity;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
    {
        if (email == null || password == null)
            return BadRequest(new { errrorText = "Недостаточно параметров" });

        try
        {
            var identity = await GetIdentityAsync(email, password);
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            
            return new JsonResult(
                new {
                    acessToken = encodedJwt,
                    user = new {
                        firstName = identity.Name,
                        lastName = identity.FindFirst(ClaimTypes.Surname)?.Value,
                        role = identity.FindFirst(ClaimTypes.Role)?.Value,
                        folder = _fileSystemService.RootId
                    }
                }
            );
        }
        catch (UserNotFoundException)
        {
            return NotFound(new { errrorText = "Пользователь не найден" });
        }
        catch (InvalidPasswordException)
        {
            return BadRequest(new { errrorText = "Неправильный пароль" });
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup(
        [FromForm] string firstName, 
        [FromForm] string lastName, 
        [FromForm] string? patronymic,
        [FromForm] string email, 
        [FromForm] string password)
    {
        if (firstName == null || lastName == null || email == null || password == null)
            return BadRequest(new { errrorText = "Недостаточно параметров" });
        if (await IsEmailUsedAsync(email))
            return BadRequest(new { errrorText = "Данная почта уже используется" });

        await _authenticationService.RegisterAsync(firstName, lastName, patronymic, email, password, 1);
        return Ok();
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PracticeWeb.Exceptions;
using PracticeWeb.Services.AuthenticationServices;
using PracticeWeb.Services.UserServices;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private IAuthenticationService _authenticationService;
    private IUserService _userService;

    public UserController(IAuthenticationService authenticationService, IUserService userService)
    {
        _authenticationService = authenticationService;
        _userService = userService;
    }

    public bool IsEmailUsed(string email)
    {
        return _userService.GetByEmailAsync(email) == null;
    }

    private async Task<ClaimsIdentity> GetIdentityAsync(string email, string password)
    {
        var user = await _authenticationService.LoginAsync(email, password);
        var claims = new List<Claim>
        {
            new Claim("Email", user.Email),
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.FirstName),
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
            return BadRequest();

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
                    acess_token = encodedJwt,
                    username = identity.Name
                }
            );
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidPasswordException)
        {
            return BadRequest();
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup(
        [FromForm] string firstName, 
        [FromForm] string lastName, 
        [FromForm] string patronymic,
        [FromForm] string email, 
        [FromForm] string password)
    {
        Console.WriteLine($"{firstName} {lastName} {patronymic} {email} {IsEmailUsed(email)} {password}");
        if (firstName == null || lastName == null || patronymic == null || email == null || IsEmailUsed(email) || password == null)
            return BadRequest();

        await _authenticationService.RegisterAsync(firstName, lastName, patronymic, email, password, 1);
        return Ok();
    }
}
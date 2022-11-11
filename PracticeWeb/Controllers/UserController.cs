using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace PracticeWeb.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
    {
        if (email == null || password == null)
            return BadRequest();

        var identity = GetIdentity(email, password);
        if (identity == null)
            return BadRequest();

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

    private ClaimsIdentity GetIdentity(string email, string password)
    {
        // db search
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, email),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, "testRole")
        };
        ClaimsIdentity identity = new ClaimsIdentity(
            claims, 
            "Token", 
            ClaimsIdentity.DefaultNameClaimType, 
            ClaimsIdentity.DefaultRoleClaimType);
        return identity;
    }
}
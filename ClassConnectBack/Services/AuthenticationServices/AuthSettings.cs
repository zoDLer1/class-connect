using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ClassConnect.Services.AuthenticationServices;

public class AuthSettings
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int Lifetime { get; set; }
    public string Key { get; set; } = null!;

    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}

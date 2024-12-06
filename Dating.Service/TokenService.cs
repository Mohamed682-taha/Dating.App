using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dating.Data.Entities;
using Dating.Data.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dating.Service;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var key = config["JWT:secretKey"] ?? "key does not exist";
        var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.UserName)
        };
        var credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = credentials
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
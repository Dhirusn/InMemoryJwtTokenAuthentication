using IdentityInMemoryTest.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityInMemoryTest.Repository
{
    public interface IJwtTokenHandler
    {
        string GenerateJSONWebToken(AppUser userInfo);
    }
    public class JwtTokenHandler : IJwtTokenHandler
    {
        public string GenerateJSONWebToken(AppUser userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JwtKeyqwertyupoiuhygtfrd"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
        new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var token = new JwtSecurityToken("JWTIssuer",
                "JWTIssuer",
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

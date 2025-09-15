using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Auth
{
    public class JwtHelper
    {
        public static string GenerateJwtToken(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("Name is not specified");
            }

            List<Claim> claims =
            [
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, "Admin")
            ];

            var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
            (
                "MyCoolServer", 
                "MyCoolClients", 
                claims, 
                expires: DateTime.Now.AddMinutes(1), 
                signingCredentials: credentials
            );

            return JwtTokenHandler.WriteToken(token);
        }

        public static JwtSecurityTokenHandler JwtTokenHandler = new();
        public static SymmetricSecurityKey SecurityKey = new(Encoding.ASCII.GetBytes("289b34bc-7381-40ca-917e-43357542fd3f"));
    }
}

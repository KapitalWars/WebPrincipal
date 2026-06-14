using Conqueco.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Conqueco.Services.Auth
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(ApplicationUser user)
        {
            var jwt = _config.GetSection("Jwt");

            var keyString = jwt["Key"];
            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];
            var durationStr = jwt["DurationInMinutes"];

            if (string.IsNullOrEmpty(keyString))
                throw new Exception("JWT Key is missing");

            if (string.IsNullOrEmpty(issuer))
                throw new Exception("JWT Issuer is missing");

            if (string.IsNullOrEmpty(audience))
                throw new Exception("JWT Audience is missing");

            if (!int.TryParse(durationStr, out var duration))
                duration = 60;

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyString)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

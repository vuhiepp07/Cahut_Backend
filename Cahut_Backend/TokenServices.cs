using Cahut_Backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cahut_Backend
{
    public static class TokenServices
    {
        public static string CreateToken(ClaimsIdentity claims)
        {
            byte[] key = Encoding.ASCII.GetBytes("asdqwuida42381dasdasd");
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }

        public static string CreateRefreshToken()
        {
            var RefreshToken = Helper.RandomString(32);
            return RefreshToken;
        }

        public static string GetUserIdFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("asdqwuida42381dasdasd")),
                ValidateLifetime = false //don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}

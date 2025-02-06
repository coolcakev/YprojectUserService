using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using y_nuget.Auth;
using YprojectUserService.UserFolder.Entities;

namespace YprojectUserService.Authorization.Services
{
    public class JWtService
    {
        private readonly AuthOptions _authOptions;
        public JWtService(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions.Value;
        }

        public string GenerateToken(string id, string email, bool isResetToken)
        {
            var claims = new List<Claim>
            {
                new("id", id),
                new("userEmail", email),
                new("isResetToken", isResetToken.ToString())
            };

            var jwt = new JwtSecurityToken(
                issuer: _authOptions.Issuer,   
                audience: _authOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(365)),
                signingCredentials: new SigningCredentials(_authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return token;
        }
    }
}
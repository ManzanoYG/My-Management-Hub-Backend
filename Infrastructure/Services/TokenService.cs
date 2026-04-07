using Infrastructure.Services.dto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class TokenService
    {
        private const double EXPIRY_DURATION_MINUTES = 15;

        public string BuildToken(string key, string issuer, DtoInputToken token)
        {
            var claims = new[] {
            new Claim("userID", token.userID.ToString()),
            new Claim("role", token.userType.ToString()),
            new Claim(ClaimTypes.NameIdentifier, token.userID.ToString())
        };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.UtcNow.AddMinutes(EXPIRY_DURATION_MINUTES), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public bool IsTokenValid(string key, string issuer, string token)
        {
            var mySecret = Encoding.UTF8.GetBytes(key);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = issuer,
                        ValidAudience = "ManagementHub",
                        IssuerSigningKey = mySecurityKey
                    }, out var validatedToken);
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return false;
            }
            return true;
        }

    }
}

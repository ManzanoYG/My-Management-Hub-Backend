using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(string username, string type)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, type),
                new Claim(JwtRegisteredClaimNames.Typ, "access")
            };

            var expirationMinutes = int.Parse(_configuration["JwtSettings:AccessTokenValidityMinutes"] ?? "15");
            return CreateSignedToken(claims, DateTime.UtcNow.AddMinutes(expirationMinutes));
        }

        public string GenerateRefreshToken(string username, string type)
        {
            var jti = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, type),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(JwtRegisteredClaimNames.Typ, "refresh")
            };

            var refreshDays = int.Parse(_configuration["JwtSettings:RefreshTokenValidityDays"] ?? "7");
            return CreateSignedToken(claims, DateTime.UtcNow.AddDays(refreshDays));
        }

        public ClaimsPrincipal ValidateExpiredAccessToken(string token)
        {
            var tokenValidationParameters = BuildTokenValidationParameters(validateLifetime: false);
            return ValidateAndGetPrincipal(token, tokenValidationParameters, "access");
        }

        public ClaimsPrincipal ValidateRefreshToken(string token)
        {
            var tokenValidationParameters = BuildTokenValidationParameters(validateLifetime: true);
            return ValidateAndGetPrincipal(token, tokenValidationParameters, "refresh");
        }

        public string ComputeTokenHash(string token)
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(hashBytes);
        }

        public DateTime GetRefreshTokenExpiryUtc()
        {
            var refreshDays = int.Parse(_configuration["JwtSettings:RefreshTokenValidityDays"] ?? "7");
            return DateTime.UtcNow.AddDays(refreshDays);
        }

        private string CreateSignedToken(IEnumerable<Claim> claims, DateTime expires)
        {
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var secretKey = _configuration["JwtSettings:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("JwtSettings:SecretKey is missing.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private TokenValidationParameters BuildTokenValidationParameters(bool validateLifetime)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("JwtSettings:SecretKey is missing.");
            }

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.Zero
            };
        }

        private static ClaimsPrincipal ValidateAndGetPrincipal(
            string token,
            TokenValidationParameters validationParameters,
            string expectedType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                throw new SecurityTokenException("Invalid JWT token.");
            }

            var tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Typ)?.Value;
            if (!string.Equals(tokenType, expectedType, StringComparison.OrdinalIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token type.");
            }

            return principal;
        }
    }
}

using Application.UseCases.Authentication;
using Application.UseCases.Authentication.Dtos;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyManagementHub_API.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        private readonly IRefreshTokenStore _refreshTokenStore;
        private readonly UseCaseLogin _useCaseLogin;

        public AuthenticationController(
            IConfiguration configuration,
            TokenService tokenService,
            IRefreshTokenStore refreshTokenStore,
            UseCaseLogin useCaseLogin)
        {
            _configuration = configuration;
            _tokenService = tokenService;
            _refreshTokenStore = refreshTokenStore;
            _useCaseLogin = useCaseLogin;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UserLogin([FromBody] DtoInputLogin login)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }

                var user = _useCaseLogin.Execute(login);

                var username = user.UserName;
                var role = user.UserType.ToString();

                var accessToken = _tokenService.GenerateAccessToken(username, role);
                var refreshToken = _tokenService.GenerateRefreshToken(username, role);

                _refreshTokenStore.Save(
                    username,
                    _tokenService.ComputeTokenHash(refreshToken),
                    _tokenService.GetRefreshTokenExpiryUtc());

                SetAccessTokenCookie(accessToken);
                SetRefreshTokenCookie(refreshToken);

                return Ok(new
                {
                    username,
                    role,
                    message = "Authenticated"
                });
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(new { e.Message });
            }
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Refresh()
        {
            try
            {
                var refreshTokenCookieName = _configuration["JwtSettings:RefreshTokenCookieName"] ?? "ManagementHubRefreshToken";
                var refreshToken = Request.Cookies[refreshTokenCookieName];

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return Unauthorized(new { Message = "Refresh token is missing." });
                }

                var principal = _tokenService.ValidateRefreshToken(refreshToken);
                var username = principal.FindFirstValue(ClaimTypes.Name);
                var role = principal.FindFirstValue(ClaimTypes.Role);

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(role))
                {
                    return Unauthorized(new { Message = "Invalid refresh token claims." });
                }

                var currentRefreshHash = _tokenService.ComputeTokenHash(refreshToken);
                if (!_refreshTokenStore.Validate(username, currentRefreshHash))
                {
                    return Unauthorized(new { Message = "Refresh token revoked or invalid." });
                }

                var newAccessToken = _tokenService.GenerateAccessToken(username, role);
                var newRefreshToken = _tokenService.GenerateRefreshToken(username, role);
                var newRefreshHash = _tokenService.ComputeTokenHash(newRefreshToken);

                _refreshTokenStore.Rotate(
                    username,
                    currentRefreshHash,
                    newRefreshHash,
                    _tokenService.GetRefreshTokenExpiryUtc());

                SetAccessTokenCookie(newAccessToken);
                SetRefreshTokenCookie(newRefreshToken);

                return Ok(new { message = "Token refreshed" });
            }
            catch (Exception)
            {
                return Unauthorized(new { Message = "Invalid or expired refresh token." });
            }
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Me()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(role))
            {
                return Unauthorized();
            }

            return Ok(new { username, role });
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            string? username = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(username))
            {
                var refreshTokenCookieName = _configuration["JwtSettings:RefreshTokenCookieName"] ?? "ManagementHubRefreshToken";
                var refreshToken = Request.Cookies[refreshTokenCookieName];

                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    try
                    {
                        var principal = _tokenService.ValidateRefreshToken(refreshToken);
                        username = principal.FindFirstValue(ClaimTypes.Name);
                    }
                    catch
                    {
                        // Ignore invalid refresh token and continue cookie cleanup.
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                _refreshTokenStore.Revoke(username);
            }

            DeleteAuthCookies();
            return Ok(new { Message = "Logged out" });
        }

        private void SetAccessTokenCookie(string token)
        {
            var accessTokenCookieName = _configuration["JwtSettings:AccessTokenCookieName"] ?? "ManagementHubAccessToken";
            var accessValidityMinutes = int.Parse(_configuration["JwtSettings:AccessTokenValidityMinutes"] ?? "15");

            Response.Cookies.Append(accessTokenCookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(accessValidityMinutes)
            });
        }

        private void SetRefreshTokenCookie(string token)
        {
            var refreshTokenCookieName = _configuration["JwtSettings:RefreshTokenCookieName"] ?? "ManagementHubRefreshToken";
            var refreshValidityDays = int.Parse(_configuration["JwtSettings:RefreshTokenValidityDays"] ?? "7");

            Response.Cookies.Append(refreshTokenCookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddDays(refreshValidityDays)
            });
        }

        private void DeleteAuthCookies()
        {
            var accessTokenCookieName = _configuration["JwtSettings:AccessTokenCookieName"] ?? "ManagementHubAccessToken";
            var refreshTokenCookieName = _configuration["JwtSettings:RefreshTokenCookieName"] ?? "ManagementHubRefreshToken";

            Response.Cookies.Delete(accessTokenCookieName);
            Response.Cookies.Delete(refreshTokenCookieName);
        }
    }
}

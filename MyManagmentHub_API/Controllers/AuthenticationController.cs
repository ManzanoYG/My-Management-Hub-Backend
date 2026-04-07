using Application.UseCases.Authentication;
using Application.UseCases.Authentication.Dtos;
using Application.UseCases.User;
using Infrastructure.Ef.Authentication;
using Infrastructure.Services;
using Infrastructure.Services.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using YamlDotNet.Core.Tokens;

namespace MyManagementHub_API.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;
        private readonly UseCaseLogin _useCaseLogin;
        private readonly IAuditService _auditService;
        private readonly UseCaseFetchUserById _useCaseFetchUserById;

        public AuthenticationController(IConfiguration configuration,TokenService tokenService,UseCaseLogin useCaseLogin,IAuditService auditService, UseCaseFetchUserById useCaseFetchUserById)
        {
            _configuration = configuration;
            _tokenService = tokenService;
            _useCaseLogin = useCaseLogin;
            _auditService = auditService;
            _useCaseFetchUserById = useCaseFetchUserById;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<DtoOutputToken> Login([FromBody][Required] DtoInputLogin login)
        {
            try
            {
                var authResult = _useCaseLogin.Execute(login);
                if (!authResult.isLogged) return BadRequest("Wrong credentials");
                return Ok(GenerateAndSetToken(new DtoInputToken
                { userID = authResult.userId, userType = authResult.usertype.ToString() }));
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(new
                {
                    e.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("generationToken")]
        public DtoOutputToken GenerateAndSetToken(DtoInputToken dto)
        {
            var token = _tokenService.BuildToken(_configuration["JwtSettings:SecretKey"]!, _configuration["JwtSettings:Issuer"]!, dto);
            HttpContext.Response.Cookies.Append("ManagementHubSession", token, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                MaxAge = TimeSpan.FromMinutes(double.Parse(_configuration["JwtSettings:AccessTokenValidityMinutes"]!)),
                IsEssential = true
            });

            return new DtoOutputToken
            {
                token = token
            };
        }

        [HttpGet("IsConnected")]
        [Authorize]
        public ActionResult IsConnected()
        {
            return Ok();
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("ManagementHubSession", new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                MaxAge = TimeSpan.FromHours(-1),
                IsEssential = true
            });

            return Ok(new { message = "Logout successful" });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst("role")?.Value ?? User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = _useCaseFetchUserById.Execute(userId);

            return Ok(new
            {
                userID = userId,
                username = user.Username,
                role = role
            });
        }
    }
}

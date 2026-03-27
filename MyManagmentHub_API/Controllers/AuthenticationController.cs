using Application.UseCases.Authentication;
using Application.UseCases.Authentication.Dtos;
using Infrastructure.Ef.Authentication;
using Infrastructure.Services;
using Infrastructure.Services.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
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

        public AuthenticationController(IConfiguration configuration,TokenService tokenService,UseCaseLogin useCaseLogin,IAuditService auditService)
        {
            _configuration = configuration;
            _tokenService = tokenService;
            _useCaseLogin = useCaseLogin;
            _auditService = auditService;
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
                { username = authResult.username, userType = authResult.usertype.ToString() }));
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

        [HttpGet("me")]
        public IActionResult Me()
        {
            var token = Request.Cookies["ManagementHubSession"];

            if (token == null)
                return Unauthorized();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? jwt.Claims.FirstOrDefault(c => c.Type.Contains("role"))?.Value;
            var username = jwt.Claims.FirstOrDefault(c => c.Type == "username")?.Value ?? jwt.Claims.FirstOrDefault(c => c.Type.Contains("username"))?.Value;

            return Ok(new
            {
                username = username,
                role = role
            });
        }
    }
}

using Application.UseCases.User;
using Application.UseCases.User.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MyManagementHub_API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly UseCaseCreateUser _useCaseCreateUser;
        private readonly UseCaseChangePassword _useCaseChangePassword;
        private readonly UseCaseDeleteUser _useCaseDeleteUser;
        private readonly UseCaseFetchUserById _useCaseFetchUserById;

        public UserController(UseCaseCreateUser useCaseCreateUser, UseCaseChangePassword useCaseChangePassword, UseCaseDeleteUser useCaseDeleteUser, UseCaseFetchUserById useCaseFetchUserById)
        {
            _useCaseCreateUser = useCaseCreateUser;
            _useCaseChangePassword = useCaseChangePassword;
            _useCaseDeleteUser = useCaseDeleteUser;
            _useCaseFetchUserById = useCaseFetchUserById;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<DtoOutputUser> Create(DtoInputCreateUser user)
        {
            var output = _useCaseCreateUser.Execute(user);
            return CreatedAtAction(
                nameof(FetchById),
                new { id = output.Id },
                output
            );
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DtoOutputUser> FetchById(Guid id)
        {
            try
            {
                return Ok(_useCaseFetchUserById.Execute(id));
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(new
                {
                    e.Message
                });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("changePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DtoOutputChangePassword> ChangePassword([FromBody] DtoInputChangePassword changePassword)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid or missing user id in token.");
            }

            var output = _useCaseChangePassword.Execute(changePassword, parsedUserId);
            if(!output.PasswordChanged)
                return NotFound(output);

            return Ok(output);
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DtoOutputDeleteUser> Delete()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid or missing user id in token.");
            }

            var result = _useCaseDeleteUser.Execute(parsedUserId);

            if (!result.Deleted)
                return NotFound(result);

            return Ok(result);
        }

    }
}

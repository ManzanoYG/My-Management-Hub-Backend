using Application.UseCases.User;
using Application.UseCases.User.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MyManagementHub_API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly UseCaseCreateUser _useCaseCreateUser;
        private readonly UseCaseFetchUserByUsername _useCaseFetchUserByUsername;
        private readonly UseCaseChangePassword _useCaseChangePassword;
        private readonly UseCaseDeleteUser _useCaseDeleteUser;

        public UserController(UseCaseCreateUser useCaseCreateUser, UseCaseFetchUserByUsername useCaseFetchUserByUsername, UseCaseChangePassword useCaseChangePassword, UseCaseDeleteUser useCaseDeleteUser)
        {
            _useCaseCreateUser = useCaseCreateUser;
            _useCaseFetchUserByUsername = useCaseFetchUserByUsername;
            _useCaseChangePassword = useCaseChangePassword;
            _useCaseDeleteUser = useCaseDeleteUser;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<DtoOutputUser> Create(DtoInputCreateUser user)
        {
            var output = _useCaseCreateUser.Execute(user);
            return CreatedAtAction(
                nameof(FetchByUsername),
                new { username = user.UserName },
                output
            );
        }

        [HttpGet]
        [Route("{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DtoOutputUser> FetchByUsername(string username)
        {
            try
            {
                return Ok(_useCaseFetchUserByUsername.Execute(username));
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(new
                {
                    e.Message
                });
            }
        }

        [HttpPut]
        [Route("changePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DtoOutputChangePassword> ChangePassword([FromBody] DtoInputChangePassword changePassword)
        {
            var output = _useCaseChangePassword.Execute(changePassword);
            if(!output.PasswordChanged)
                return NotFound(output);

            return Ok(output);
        }

        [HttpDelete("{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DtoOutputDeleteUser> Delete(string username)
        {
            var result = _useCaseDeleteUser.Execute(new DtoInputDeleteUser
            {
                Username = username
            });

            if (!result.Deleted)
                return NotFound(result);

            return Ok(result);
        }

    }
}

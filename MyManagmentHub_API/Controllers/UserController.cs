using Application.UseCases.User;
using Application.UseCases.User.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MyManagementHub_API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly UseCaseCreateUser _useCaseCreateUser;
        private readonly UseCaseFetchUserByUsername _useCaseFetchUserByUsername;

        public UserController(UseCaseCreateUser useCaseCreateUser, UseCaseFetchUserByUsername useCaseFetchUserByUsername)
        {
            _useCaseCreateUser = useCaseCreateUser;
            _useCaseFetchUserByUsername = useCaseFetchUserByUsername;
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

    }
}

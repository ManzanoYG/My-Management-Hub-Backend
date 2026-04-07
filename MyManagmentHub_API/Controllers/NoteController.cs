using Application.UseCases.Note;
using Application.UseCases.Note.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace MyManagementHub_API.Controllers
{
    [ApiController]
    [Route("api/note")]
    public class NoteController : ControllerBase
    {
        private readonly UseCaseCreateNote _useCaseCreateNote;
        private readonly UseCaseGetAllNotePinned _useCaseGetAllNotePinned;

        public NoteController(UseCaseCreateNote useCaseCreateNote, UseCaseGetAllNotePinned useCaseGetAllNotePinned)
        {
            _useCaseCreateNote = useCaseCreateNote;
            _useCaseGetAllNotePinned = useCaseGetAllNotePinned;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<DtoOutputCreateNote> Create(DtoInputCreateNote note)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized("Invalid or missing user id in token.");
            }

            var output = _useCaseCreateNote.Execute(note, parsedUserId);
            
            return output;
        }

        [HttpGet("pinned")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<DtoOutputGetAllPinned>> GetAllPinned(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var parsedUserId))
                {
                    return Unauthorized("Invalid or missing user id in token.");
                }

                var dto = new DtoInputGetAllPinned
                {
                    userId = parsedUserId,
                    pageNumber = pageNumber,
                    pageSize = pageSize
                };

                return Ok(_useCaseGetAllNotePinned.Execute(dto));
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

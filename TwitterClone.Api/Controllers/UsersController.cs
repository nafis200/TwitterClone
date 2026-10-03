using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitterClone.Application.Dtos;
using TwitterClone.Application.Interfaces;

namespace TwitterClone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UsersController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        // GET /api/users
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_userServices.GetUsers());
        }

        // GET /api/users/{id}
        [HttpGet("{id}")]
        public IActionResult GetUserById([FromRoute] Guid id)
        {
            var user = _userServices.GetUserById(id);
            if (user is null)
            {
                return NotFound(new { Message = $"User with ID '{id}' was not found." });
            }

            return Ok(user);
        }

        // POST /api/users
        [HttpPost]
        [AllowAnonymous]
        public IActionResult CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var createdUser = _userServices.CreateUser(createUserDto);
            if (createdUser is null)
            {
                return BadRequest(new { Message = "First name, last name, and a unique email are required." });
            }

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        // PUT /api/users/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (_userServices.GetUserById(id) is null)
            {
                return NotFound(new { Message = $"User with ID '{id}' was not found." });
            }

            var updatedUser = _userServices.UpdateUser(id, updateUserDto);
            if (updatedUser is null)
            {
                return BadRequest(new { Message = "First name, last name, and a unique email are required." });
            }

            return Ok(updatedUser);
        }

        // DELETE /api/users/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUser([FromRoute] Guid id)
        {
            if (!_userServices.DeleteUser(id))
            {
                return NotFound(new { Message = $"User with ID '{id}' was not found." });
            }

            return Ok(new
            {
                UserId = id,
                Message = "User deleted successfully."
            });
        }
    }
}

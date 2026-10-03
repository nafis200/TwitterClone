using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitterClone.Api.Data;
using TwitterClone.Application.Dtos;
using TwitterClone.Domain.Entities;

namespace TwitterClone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UsersController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET /api/users
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_userRepository.GetAllUsers());
        }

        // GET /api/users/{id}
        [HttpGet("{id}")]
        public IActionResult GetUserById([FromRoute] Guid id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
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
            if (string.IsNullOrWhiteSpace(createUserDto.FirstName) ||
                string.IsNullOrWhiteSpace(createUserDto.LastName) ||
                string.IsNullOrWhiteSpace(createUserDto.Email))
            {
                return BadRequest(new { Message = "First name, last name, and email are required." });
            }

            // Check if email already exists
            var existingUser = _userRepository.GetUserByEmail(createUserDto.Email);
            if (existingUser != null)
            {
                return Conflict(new { Message = "User with this email already exists." });
            }

            var newUser = new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email
            };

            var createdUser = _userRepository.AddUser(newUser);

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        // PUT /api/users/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateUser([FromRoute] Guid id, [FromBody] CreateUserDto updateUserDto)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with ID '{id}' was not found." });
            }

            if (string.IsNullOrWhiteSpace(updateUserDto.FirstName) ||
                string.IsNullOrWhiteSpace(updateUserDto.LastName) ||
                string.IsNullOrWhiteSpace(updateUserDto.Email))
            {
                return BadRequest(new { Message = "First name, last name, and email are required." });
            }

            // If the email is changing, verify the new email is not taken by another user
            if (!string.Equals(user.Email, updateUserDto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingUserWithEmail = _userRepository.GetUserByEmail(updateUserDto.Email);
                if (existingUserWithEmail != null)
                {
                    return Conflict(new { Message = "User with this email already exists." });
                }
            }

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Email = updateUserDto.Email;

            var updatedUser = _userRepository.UpdateUser(user);
            return Ok(updatedUser);
        }



        // DELETE /api/users/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUser([FromRoute] Guid id)
        {

            return Ok(new
            {
                UserId = id,
                Message = "User deleted successfully."
            });
        }
    }
}
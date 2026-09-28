using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitterClone.Api.Data;
using TwitterClone.Api.Dtos;
using TwitterClone.Domain.Entities;

namespace TwitterClone.Api.Controllers
{

    // api/users
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]


    public class UsersController : ControllerBase
    {

        private readonly UserRepository _userRepository;
        public UsersController(
    UserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        // /api/users
        [HttpGet]
        public IActionResult GetUsers()
        {
            

            return Ok(_userRepository.GetAllUsers());
        }

        // /api/users
        [HttpPost]
        [AllowAnonymous]
        public IActionResult CreateUser([FromBody] CreateUserDto CreateUserDto)
        {
            if (string.IsNullOrWhiteSpace(CreateUserDto.FirstName) ||
        string.IsNullOrWhiteSpace(CreateUserDto.LastName) ||
        string.IsNullOrWhiteSpace(CreateUserDto.Email))
            {
                return BadRequest("All fields are required.");
            }
            

            var createUser = _userRepository.AddUser(new User
            {
                FirstName = CreateUserDto.FirstName,
                LastName = CreateUserDto.LastName,
                Email = CreateUserDto.Email
            });
            return Ok(createUser);
        }


        // /api/users/{id}
        [HttpGet("{id}")]
        public IActionResult GetUserById([FromRoute] Guid id)
        {
            return Ok(new
            {
                UserId = id,
                UserName = "user" + id.ToString(),
            });
        }


        // PUT /api/users/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateUser([FromRoute] Guid id)
        {
            return Ok(new
            {
                UserId = id,
                UserName = "updateduser" + id.ToString(),
            });
        }


        // PATCH /api/users/{id}/phoneNumber
        [HttpPatch("{id}/phoneNumber")]
        public IActionResult UpdateUserPhoneNumber([FromRoute] Guid id, [FromBody] string phoneNumber)
        {
            return Ok("hello");

        }

        // DELETE /api/users/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUser([FromRoute] Guid id)
        {
            return Ok(new
            {
                UserId = id,
                Message = "User deleted successfully.",
            });
        }
    }
}

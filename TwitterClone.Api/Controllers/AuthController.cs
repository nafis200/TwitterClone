using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitterClone.Api.Auth;
using TwitterClone.Application.Dtos;
using TwitterClone.Application.Interfaces;

namespace TwitterClone.Api.Controllers
{
    // api/auth
    // register, login and refresh are public. The others need a valid access token.
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Register([FromBody] CreateUserDto createUserDto)
        {
            var authResponse = _authService.Register(createUserDto);

            if (authResponse is null)
            {
                return Conflict($"A user with email '{createUserDto.Email}' already exists.");
            }

            return Created($"/api/users/{authResponse.User.Id}", authResponse);
        }

        // POST /api/auth/login
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var authResponse = _authService.Login(loginDto);

            if (authResponse is null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(authResponse);
        }

        // POST /api/auth/refresh
        // Trades a refresh token for a new access token (and a new refresh token).
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Refresh([FromBody] RefreshTokenRequestDto refreshTokenRequestDto)
        {
            var authResponse = _authService.Refresh(refreshTokenRequestDto.RefreshToken);

            if (authResponse is null)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            return Ok(authResponse);
        }

        // POST /api/auth/logout
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Logout([FromBody] RefreshTokenRequestDto refreshTokenRequestDto)
        {
            _authService.Logout(User.GetUserId(), refreshTokenRequestDto.RefreshToken);

            return NoContent();
        }

        // GET /api/auth/me
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Me()
        {
            // User.GetUserId() reads the "sub" claim from the access token.
            var user = _userService.GetUserById(User.GetUserId());

            if (user is null)
            {
                // The token is valid but the account has since been deleted.
                return NotFound();
            }

            return Ok(user);
        }

        // POST /api/auth/change-password
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var authResponse = _authService.ChangePassword(User.GetUserId(), changePasswordDto);

            if (authResponse is null)
            {
                return BadRequest("The current password is incorrect.");
            }

            return Ok(authResponse);
        }
    }
}

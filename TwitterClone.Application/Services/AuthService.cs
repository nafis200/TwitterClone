using TwitterClone.Application.Dtos;
using TwitterClone.Application.Interfaces;
using TwitterClone.Application.Mappings;

namespace TwitterClone.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public AuthResponseDto? Login(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return null;
            }

            var user = _userRepository.GetUserByEmail(loginDto.Email);

            // Unknown email: fail the same way a wrong password does, so the
            // response does not reveal which accounts exist.
            if (user is null)
            {
                return null;
            }

            var userAuthorized = _passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash);

            if (!userAuthorized)
            {
                return null;
            }

            var (token, expiryTime) = _tokenService.GetToken(user.ToDto());

            return new AuthResponseDto
            {
                AccessToken = token,
                ExpiryAt = expiryTime,
                UserDto = user.ToDto()
            };
        }
    }
}

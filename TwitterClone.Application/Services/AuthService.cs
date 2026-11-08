using TwitterClone.Application.Dtos;
using TwitterClone.Application.Interfaces;
using TwitterClone.Application.Mappings;
using TwitterClone.Domain.Entities;

namespace TwitterClone.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUserRepository userRepository,
            IUserService userService,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _userService = userService;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public AuthResponseDto? Register(CreateUserDto createUserDto)
        {
            var user = _userService.CreateUser(createUserDto);

            return user is null ? null : IssueTokens(user);
        }

        public AuthResponseDto? Login(LoginDto loginDto)
        {
            var user = _userRepository.GetUserByEmail(loginDto.Email);

            if (user is null)
            {
                // Spend the same time hashing as a real check would, so response time
                // does not reveal which emails are registered.
                _passwordHasher.Hash(UnknownUser, loginDto.Password);
                return null;
            }

            var userDto = user.ToDto();

            if (!_passwordHasher.VerifyPassword(userDto, loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            return IssueTokens(userDto);
        }

        public AuthResponseDto? Refresh(string refreshToken)
        {
            var storedToken = _refreshTokenRepository.GetByTokenHash(_tokenService.HashRefreshToken(refreshToken));

            if (storedToken is null)
            {
                return null;
            }

            if (storedToken.IsRevoked)
            {
                // A token that was already rotated is being used again: either the client or an
                // attacker holds a stolen copy. Sign the user out everywhere to be safe.
                // (Tokens revoked by logout or a password change are simply rejected.)
                if (storedToken.ReplacedByTokenHash != null)
                {
                    _refreshTokenRepository.RevokeAllForUser(storedToken.UserId);
                }

                return null;
            }

            if (storedToken.IsExpired)
            {
                return null;
            }

            var user = _userRepository.GetUserById(storedToken.UserId);

            if (user is null)
            {
                return null;
            }

            // Rotation: every refresh token is single-use and is replaced by a new one.
            var response = IssueTokens(user.ToDto());
            storedToken.Revoke(_tokenService.HashRefreshToken(response.RefreshToken));
            _refreshTokenRepository.UpdateRefreshToken(storedToken);

            return response;
        }

        public void Logout(Guid userId, string refreshToken)
        {
            var storedToken = _refreshTokenRepository.GetByTokenHash(_tokenService.HashRefreshToken(refreshToken));

            if (storedToken is null || storedToken.UserId != userId || storedToken.IsRevoked)
            {
                return;
            }

            storedToken.Revoke();
            _refreshTokenRepository.UpdateRefreshToken(storedToken);
        }

        public AuthResponseDto? ChangePassword(Guid userId, ChangePasswordDto changePasswordDto)
        {
            var user = _userRepository.GetUserById(userId);

            if (user is null)
            {
                return null;
            }

            var userDto = user.ToDto();

            if (!_passwordHasher.VerifyPassword(userDto, changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return null;
            }

            user.PasswordHash = _passwordHasher.Hash(userDto, changePasswordDto.NewPassword);
            user.MarkAsModified();
            _userRepository.UpdateUser(user);

            // Sign out every other session, then give this client a fresh session.
            _refreshTokenRepository.RevokeAllForUser(userId);

            return IssueTokens(user.ToDto());
        }

        private AuthResponseDto IssueTokens(UserDto user)
        {
            var (accessToken, accessTokenExpiresAt) = _tokenService.GetToken(user);
            var (refreshToken, refreshTokenExpiresAt) = _tokenService.GenerateRefreshToken();

            _refreshTokenRepository.AddRefreshToken(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashRefreshToken(refreshToken),
                ExpiresAt = refreshTokenExpiresAt
            });

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshTokenExpiresAt,
                User = user
            };
        }

        private static readonly UserDto UnknownUser = new UserDto
        {
            Id = Guid.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty
        };
    }
}

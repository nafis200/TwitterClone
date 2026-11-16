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
            // CreateUser hashes the password and saves the user. Null = email already taken.
            var user = _userService.CreateUser(createUserDto);

            if (user is null)
            {
                return null;
            }

            return IssueTokens(user);
        }

        public AuthResponseDto? Login(LoginDto loginDto)
        {
            var user = _userRepository.GetUserByEmail(loginDto.Email);

            if (user is null || !_passwordHasher.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            return IssueTokens(user.ToDto());
        }

        public AuthResponseDto? Refresh(string refreshToken)
        {
            var storedToken = _refreshTokenRepository.GetByTokenHash(_tokenService.HashRefreshToken(refreshToken));

            if (storedToken is null || !storedToken.IsActive)
            {
                return null;
            }

            var user = _userRepository.GetUserById(storedToken.UserId);

            if (user is null)
            {
                return null;
            }

            // Each refresh token works once: revoke the old one and hand out a new pair.
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

            if (user is null || !_passwordHasher.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return null;
            }

            user.PasswordHash = _passwordHasher.Hash(changePasswordDto.NewPassword);
            user.MarkAsModified();
            _userRepository.UpdateUser(user);

            // Sign out every other session, then give this client a fresh session.
            _refreshTokenRepository.RevokeAllForUser(userId);

            return IssueTokens(user.ToDto());
        }

        // Creates an access token (JWT) + a refresh token, and stores the refresh token's hash.
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
    }
}

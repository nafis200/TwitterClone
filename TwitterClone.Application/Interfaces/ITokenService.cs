using TwitterClone.Application.Dtos;

namespace TwitterClone.Application.Interfaces
{
    public interface ITokenService
    {
        // Signed JWT access token for the user.
        (string Token, DateTime ExpiresAt) GetToken(UserDto user);

        // Random opaque refresh token. The raw value goes to the client; only its hash is stored.
        (string Token, DateTime ExpiresAt) GenerateRefreshToken();

        string HashRefreshToken(string refreshToken);
    }
}

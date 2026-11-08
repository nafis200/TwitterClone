using TwitterClone.Application.Dtos;

namespace TwitterClone.Application.Interfaces
{
    public interface IAuthService
    {
        // Returns null when the email is already taken.
        AuthResponseDto? Register(CreateUserDto createUserDto);

        // Returns null when the email or password is wrong.
        AuthResponseDto? Login(LoginDto loginDto);

        // Returns null when the refresh token is unknown, expired or revoked.
        AuthResponseDto? Refresh(string refreshToken);

        // Revokes the refresh token if it belongs to the user. Safe to call repeatedly.
        void Logout(Guid userId, string refreshToken);

        // Returns null when the current password is wrong. Signs out all other sessions on success.
        AuthResponseDto? ChangePassword(Guid userId, ChangePasswordDto changePasswordDto);
    }
}

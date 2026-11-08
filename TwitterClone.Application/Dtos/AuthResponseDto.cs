namespace TwitterClone.Application.Dtos
{
    public class AuthResponseDto
    {
        public string TokenType => "Bearer";
        public required string AccessToken { get; set; }
        public required DateTime AccessTokenExpiresAt { get; set; }
        public required string RefreshToken { get; set; }
        public required DateTime RefreshTokenExpiresAt { get; set; }
        public required UserDto User { get; set; }
    }
}

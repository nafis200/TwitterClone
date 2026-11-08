using System.Text;

namespace TwitterClone.Infrastructure.Auth
{
    // Bound from the "JwtSettings" section of appsettings.
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int ExpiryTimeInMinutes { get; set; } = 15;
        public int RefreshTokenExpiryTimeInDays { get; set; } = 7;

        // Fail at startup instead of on the first login.
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Issuer))
                throw new InvalidOperationException($"{SectionName}:Issuer is not configured.");

            if (string.IsNullOrWhiteSpace(Audience))
                throw new InvalidOperationException($"{SectionName}:Audience is not configured.");

            // HMAC-SHA256 needs a key of at least 256 bits (32 bytes).
            if (Encoding.UTF8.GetByteCount(Key) < 32)
                throw new InvalidOperationException($"{SectionName}:Key must be at least 32 bytes long.");

            if (ExpiryTimeInMinutes <= 0 || RefreshTokenExpiryTimeInDays <= 0)
                throw new InvalidOperationException($"{SectionName} expiry times must be positive.");
        }
    }
}

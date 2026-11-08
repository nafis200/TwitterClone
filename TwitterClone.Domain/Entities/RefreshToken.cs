namespace TwitterClone.Domain.Entities
{
    // A long-lived token that lets a client get a new access token without logging in again.
    // Only a hash of the token is stored, so a database leak does not leak usable tokens.
    public class RefreshToken : BaseEntity
    {
        public RefreshToken() : base(Guid.NewGuid())
        {
        }

        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        // Set when this token was rotated, to trace which token replaced it.
        public string? ReplacedByTokenHash { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        public void Revoke(string? replacedByTokenHash = null)
        {
            RevokedAt = DateTime.UtcNow;
            ReplacedByTokenHash = replacedByTokenHash;
            MarkAsModified();
        }
    }
}

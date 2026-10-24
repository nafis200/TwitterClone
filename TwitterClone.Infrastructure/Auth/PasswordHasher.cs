using Microsoft.AspNetCore.Identity;
using TwitterClone.Application.Interfaces;

namespace TwitterClone.Infrastructure.Auth
{
    /// <summary>
    /// Hashes and verifies passwords using PBKDF2 (via ASP.NET Core Identity's
    /// <see cref="Microsoft.AspNetCore.Identity.PasswordHasher{TUser}"/>).
    /// The generated hash embeds its own salt, so the same password produces a
    /// different hash on every call while still verifying correctly.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _passwordHasher = new();

        public string HashPassword(string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            return _passwordHasher.HashPassword(null!, password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            // A user registered before hashing was introduced (or a tampered record)
            // can have no hash at all. Treat that as a failed verification rather
            // than throwing, so the caller simply returns 401.
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            var result = _passwordHasher.VerifyHashedPassword(null!, passwordHash, password);

            // Rehash when the stored hash uses outdated parameters.
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                return true;
            }

            return result == PasswordVerificationResult.Success;
        }
    }
}

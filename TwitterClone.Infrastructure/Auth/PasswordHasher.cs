using Microsoft.AspNetCore.Identity;
using TwitterClone.Application.Interfaces;
using TwitterClone.Domain.Entities;

namespace TwitterClone.Infrastructure.Auth
{
    // Uses ASP.NET Core Identity's hasher (PBKDF2 + random salt) so we never store plain passwords.
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

        public string Hash(string password)
        {
            // The default hasher does not use the user argument, so null is fine here.
            return _hasher.HashPassword(null!, password);
        }

        public bool Verify(string password, string passwordHash)
        {
            var result = _hasher.VerifyHashedPassword(null!, passwordHash, password);

            return result != PasswordVerificationResult.Failed;
        }
    }
}

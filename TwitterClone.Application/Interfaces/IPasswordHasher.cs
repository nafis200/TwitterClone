namespace TwitterClone.Application.Interfaces
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a plain-text password using a salted, one-way algorithm.
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a plain-text password against a previously produced hash.
        /// </summary>
        bool VerifyPassword(string password, string passwordHash);
    }
}

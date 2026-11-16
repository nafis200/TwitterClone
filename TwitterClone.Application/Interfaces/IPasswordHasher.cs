namespace TwitterClone.Application.Interfaces
{
    public interface IPasswordHasher
    {
        // Turns a plain password into a one-way hash that is safe to store in the database.
        string Hash(string password);

        // True when the plain password matches the stored hash.
        bool Verify(string password, string passwordHash);
    }
}

using TwitterClone.Application.Interfaces;
using TwitterClone.Domain.Entities;

namespace TwitterClone.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public List<User> GetAllUsers()
        {
            return _users;
        }

        public User? GetUserById(Guid id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public User? GetUserByEmail(string email)
        {
            return _users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }

        public User AddUser(User user)
        {
            _users.Add(user);
            return user;
        }

        public User UpdateUser(User user)
        {
            _users.RemoveAll(u => u.Id == user.Id);
            _users.Add(user);
            return user;
        }

        public bool DeleteUser(User user)
        {
            return _users.Remove(user);
        }
    }
}

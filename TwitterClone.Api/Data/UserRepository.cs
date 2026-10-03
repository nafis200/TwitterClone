using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterClone.Domain.Entities;

namespace TwitterClone.Api.Data
{
    public class UserRepository
    {
        private List<User> _users { get; set; } = new();

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
            return _users.FirstOrDefault(u => u.Email == email);
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
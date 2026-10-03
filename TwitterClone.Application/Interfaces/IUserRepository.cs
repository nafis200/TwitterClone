using TwitterClone.Domain.Entities;

namespace TwitterClone.Application.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAllUsers();
        User? GetUserById(Guid id);
        User? GetUserByEmail(string email);
        User AddUser(User user);
        User UpdateUser(User user);
        bool DeleteUser(User user);
    }
}

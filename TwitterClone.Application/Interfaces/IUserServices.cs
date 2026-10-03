using TwitterClone.Application.Dtos;

namespace TwitterClone.Application.Interfaces
{
    public interface IUserServices
    {
        List<UserDto> GetUsers();
        UserDto? GetUserById(Guid id);
        UserDto? CreateUser(CreateUserDto createUserDto);
        UserDto? UpdateUser(Guid id, UpdateUserDto updateUserDto);
        bool DeleteUser(Guid id);
    }
}

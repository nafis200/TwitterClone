using TwitterClone.Application.Dtos;
using TwitterClone.Application.Interfaces;
using TwitterClone.Domain.Entities;

namespace TwitterClone.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;

        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<UserDto> GetUsers()
        {
            return _userRepository.GetAllUsers().Select(ToDto).ToList();
        }

        public UserDto? GetUserById(Guid id)
        {
            var user = _userRepository.GetUserById(id);
            return user is null ? null : ToDto(user);
        }

        public UserDto? CreateUser(CreateUserDto createUserDto)
        {
            if (string.IsNullOrWhiteSpace(createUserDto.FirstName) ||
                string.IsNullOrWhiteSpace(createUserDto.LastName) ||
                string.IsNullOrWhiteSpace(createUserDto.Email))
            {
                return null;
            }

            // Email must be unique
            if (_userRepository.GetUserByEmail(createUserDto.Email) is not null)
            {
                return null;
            }

            var createdUser = _userRepository.AddUser(new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email
            });

            return ToDto(createdUser);
        }

        public UserDto? UpdateUser(Guid id, UpdateUserDto updateUserDto)
        {
            var user = _userRepository.GetUserById(id);
            if (user is null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(updateUserDto.FirstName) ||
                string.IsNullOrWhiteSpace(updateUserDto.LastName) ||
                string.IsNullOrWhiteSpace(updateUserDto.Email))
            {
                return null;
            }

            // If the email is changing, verify the new email is not taken by another user
            if (!string.Equals(user.Email, updateUserDto.Email, StringComparison.OrdinalIgnoreCase) &&
                _userRepository.GetUserByEmail(updateUserDto.Email) is not null)
            {
                return null;
            }

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Email = updateUserDto.Email;

            return ToDto(_userRepository.UpdateUser(user));
        }

        public bool DeleteUser(Guid id)
        {
            var user = _userRepository.GetUserById(id);
            return user is not null && _userRepository.DeleteUser(user);
        }

        public UserDto? UpdateUserPhoneNumber(Guid id, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return null;
            }

            var user = _userRepository.GetUserById(id);
            if (user is null)
            {
                return null;
            }

            user.PhoneNumber = phoneNumber;
            return ToDto(_userRepository.UpdateUser(user));
        }

        private static UserDto ToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }
    }
}

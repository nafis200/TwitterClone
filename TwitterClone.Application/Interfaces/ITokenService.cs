using TwitterClone.Application.Dtos;

namespace TwitterClone.Application.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Creates a signed JSON Web Token for the given user and returns it
        /// together with the moment the token expires.
        /// </summary>
        (string Token, DateTimeOffset ExpiryAt) GetToken(UserDto user);
    }
}

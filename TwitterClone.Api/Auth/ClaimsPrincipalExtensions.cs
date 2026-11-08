using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TwitterClone.Api.Auth
{
    public static class ClaimsPrincipalExtensions
    {
        // The id of the logged-in user, read from the access token's "sub" claim.
        // Only call this on endpoints that require authentication.
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var subject = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!Guid.TryParse(subject, out var userId))
            {
                throw new InvalidOperationException("The access token does not contain a valid user id.");
            }

            return userId;
        }
    }
}

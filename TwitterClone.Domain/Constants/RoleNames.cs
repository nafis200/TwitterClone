namespace TwitterClone.Domain.Constants
{
    public static class RoleNames
    {
        // The claim name we use for roles inside the JWT.
        public const string ClaimType = "role";

        public const string Admin = "Admin";
        public const string Moderator = "Moderator";
        public const string User = "User";
    }
}
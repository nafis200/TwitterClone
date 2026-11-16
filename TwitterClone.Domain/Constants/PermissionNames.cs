namespace TwitterClone.Domain.Constants
{
    public static class PermissionNames
    {
        // The claim name we use for permissions inside the JWT.
        public const string ClaimType = "permission";

        public const string UsersRead = "users.read";
        public const string UsersDelete = "users.delete";
        public const string RolesManage = "roles.manage";

        // Every permission the API checks. Program.cs registers one rule per entry.
        public static readonly string[] All = { UsersRead, UsersDelete, RolesManage };
    }
}
using TwitterClone.Domain.Constants;
using TwitterClone.Domain.Entities;

namespace TwitterClone.Infrastructure.Persistence.Seed
{
    // Fixed lookup rows used by HasData(). IDs must never change once a migration has been applied.
    public static class SeedData
    {
        public static class RoleIds
        {
            public const int Admin = 1;
            public const int Moderator = 2;
            public const int User = 3;
        }

        public static class PermissionIds
        {
            public const int UsersRead = 1;
            public const int UsersDelete = 2;
            public const int RolesManage = 3;
        }

        public static readonly Role[] Roles =
        {
            new Role { Id = RoleIds.Admin, Name = RoleNames.Admin },
            new Role { Id = RoleIds.Moderator, Name = RoleNames.Moderator },
            new Role { Id = RoleIds.User, Name = RoleNames.User }
        };

        public static readonly Permission[] Permissions =
        {
            new Permission { Id = PermissionIds.UsersRead, Name = PermissionNames.UsersRead },
            new Permission { Id = PermissionIds.UsersDelete, Name = PermissionNames.UsersDelete },
            new Permission { Id = PermissionIds.RolesManage, Name = PermissionNames.RolesManage }
        };

        public static readonly RolePermission[] RolePermissions =
        {
            // Admin: everything
            new RolePermission { RoleId = RoleIds.Admin, PermissionId = PermissionIds.UsersRead },
            new RolePermission { RoleId = RoleIds.Admin, PermissionId = PermissionIds.UsersDelete },
            new RolePermission { RoleId = RoleIds.Admin, PermissionId = PermissionIds.RolesManage },

            // Moderator: read and delete users
            new RolePermission { RoleId = RoleIds.Moderator, PermissionId = PermissionIds.UsersRead },
            new RolePermission { RoleId = RoleIds.Moderator, PermissionId = PermissionIds.UsersDelete }

            // User: no special permissions
        };
    }
}

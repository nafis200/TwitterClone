namespace TwitterClone.Domain.Entities
{
    // Lookup table with fixed int IDs so rows can be seeded deterministically (see SeedData).
    public class Role
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}

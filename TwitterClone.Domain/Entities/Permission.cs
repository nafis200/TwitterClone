namespace TwitterClone.Domain.Entities
{
    // Lookup table with fixed int IDs so rows can be seeded deterministically (see SeedData).
    public class Permission
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}

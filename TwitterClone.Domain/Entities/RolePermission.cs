namespace TwitterClone.Domain.Entities
{
    // One row = "this role has this permission". Key is (RoleId, PermissionId).
    public class RolePermission
    {
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
    }
}

using TwitterClone.Application.Interfaces;
using TwitterClone.Domain.Entities;
using TwitterClone.Infrastructure.Persistence;

namespace TwitterClone.Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly TwitterCloneDbContext _context;

        public PermissionRepository(TwitterCloneDbContext context)
        {
            _context = context;
        }

        public Permission? GetPermissionByName(string name)
        {
            return _context.Permissions.SingleOrDefault(p => p.Name == name);
        }
    }
}
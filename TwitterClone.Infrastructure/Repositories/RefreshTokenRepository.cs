using TwitterClone.Application.Interfaces;
using TwitterClone.Domain.Entities;
using TwitterClone.Infrastructure.Persistence;

namespace TwitterClone.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly TwitterCloneDbContext _context;

        public RefreshTokenRepository(TwitterCloneDbContext context)
        {
            _context = context;
        }

        public RefreshToken AddRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();

            return refreshToken;
        }

        public RefreshToken? GetByTokenHash(string tokenHash)
        {
            return _context.RefreshTokens.SingleOrDefault(t => t.TokenHash == tokenHash);
        }

        public RefreshToken UpdateRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            _context.SaveChanges();

            return refreshToken;
        }

        public void RevokeAllForUser(Guid userId)
        {
            var activeTokens = _context.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedAt == null)
                .ToList();

            foreach (var token in activeTokens)
            {
                token.Revoke();
            }

            _context.SaveChanges();
        }
    }
}

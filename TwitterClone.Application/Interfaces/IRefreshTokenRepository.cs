using TwitterClone.Domain.Entities;

namespace TwitterClone.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        RefreshToken AddRefreshToken(RefreshToken refreshToken);
        RefreshToken? GetByTokenHash(string tokenHash);
        RefreshToken UpdateRefreshToken(RefreshToken refreshToken);
        void RevokeAllForUser(Guid userId);
    }
}

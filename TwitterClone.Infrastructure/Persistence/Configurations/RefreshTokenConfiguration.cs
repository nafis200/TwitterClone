using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitterClone.Domain.Entities;

namespace TwitterClone.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(t => t.Id);

            // SHA-256 as hex is 64 characters.
            builder.Property(t => t.TokenHash).IsRequired().HasMaxLength(64);
            builder.Property(t => t.ReplacedByTokenHash).HasMaxLength(64);

            builder.HasIndex(t => t.TokenHash).IsUnique();
            builder.HasIndex(t => t.UserId);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(t => t.IsExpired);
            builder.Ignore(t => t.IsRevoked);
            builder.Ignore(t => t.IsActive);
        }
    }
}

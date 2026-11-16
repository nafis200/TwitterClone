using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitterClone.Domain.Entities;

namespace TwitterClone.Infrastructure.Persistence.Configurations
{
    // Tells EF Core how the Tweet class maps to the "Tweets" table.
    public class TweetConfiguration : IEntityTypeConfiguration<Tweet>
    {
        public void Configure(EntityTypeBuilder<Tweet> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Content).IsRequired().HasMaxLength(Tweet.MaxContentLength);

            builder.HasIndex(t => t.UserId);

            // Every tweet belongs to a user. Deleting the user deletes their tweets.
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

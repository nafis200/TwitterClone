using TwitterClone.Application.Interfaces;
using TwitterClone.Domain.Entities;
using TwitterClone.Infrastructure.Persistence;

namespace TwitterClone.Infrastructure.Repositories
{
    // Reads and writes the "Tweets" table through EF Core.
    public class TweetRepository : ITweetRepository
    {
        private readonly TwitterCloneDbContext _context;

        public TweetRepository(TwitterCloneDbContext context)
        {
            _context = context;
        }

        public Tweet AddTweet(Tweet tweet)
        {
            _context.Tweets.Add(tweet);   // INSERT is prepared...
            _context.SaveChanges();       // ...and sent to the database here.

            return tweet;
        }

        public Tweet UpdateTweet(Tweet tweet)
        {
            _context.Tweets.Update(tweet);
            _context.SaveChanges();

            return tweet;
        }

        public bool DeleteTweet(Tweet tweet)
        {
            _context.Tweets.Remove(tweet);

            return _context.SaveChanges() > 0;
        }

        public int DeleteTweetsByUserId(Guid userId)
        {
            var tweets = _context.Tweets.Where(t => t.UserId == userId).ToList();
            _context.Tweets.RemoveRange(tweets);

            return _context.SaveChanges();
        }

        public Tweet? GetTweetById(Guid id)
        {
            return _context.Tweets.Find(id);
        }

        public List<Tweet> GetTweets()
        {
            return _context.Tweets.ToList();
        }

        public List<Tweet> GetTweetsByUserId(Guid userId)
        {
            return _context.Tweets.Where(t => t.UserId == userId).ToList();
        }
    }
}

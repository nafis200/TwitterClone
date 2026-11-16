using TwitterClone.Application.Dtos;

namespace TwitterClone.Application.Interfaces
{
    public interface ITweetService
    {
        // Returns null when the user does not exist.
        TweetDto? CreateTweet(Guid userId, CreateTweetDto createTweetDto);
        TweetDto? GetTweetById(Guid id);
        List<TweetDto> GetTweets(Guid? userId);
        TweetDto? UpdateTweet(Guid id, UpdateTweetDto updateTweetDto);
        bool DeleteTweet(Guid id);
    }
}

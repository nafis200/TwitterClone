using TwitterClone.Application.Dtos;

namespace TwitterClone.Application.Interfaces
{
    public interface ITweetService
    {
        List<TweetDto> GetTweets(Guid? userId);
        TweetDto? GetTweetById(Guid id);
        TweetDto? CreateTweet(CreateTweetDto createTweetDto);
        TweetDto? UpdateTweet(Guid id, UpdateTweetDto updateTweetDto);
        bool DeleteTweet(Guid id);
    }
}
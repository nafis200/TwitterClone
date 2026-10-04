using TwitterClone.Application.Dtos;
using TwitterClone.Application.Interfaces;
using TwitterClone.Domain.Entities;

namespace TwitterClone.Application.Services
{
    public class TweetService : ITweetService
    {
        private readonly ITweetRepository _tweetRepository;
        private readonly IUserRepository _userRepository;

        public TweetService(ITweetRepository tweetRepository, IUserRepository userRepository)
        {
            _tweetRepository = tweetRepository;
            _userRepository = userRepository;
        }

        public List<TweetDto> GetTweets(Guid? userId)
        {
            var tweets = userId.HasValue
                ? _tweetRepository.GetTweetsByUserId(userId.Value)
                : _tweetRepository.GetTweets();

            return tweets.Select(ToDto).ToList();
        }

        public TweetDto? GetTweetById(Guid id)
        {
            var tweet = _tweetRepository.GetTweetById(id);
            return tweet is null ? null : ToDto(tweet);
        }

        public TweetDto? CreateTweet(CreateTweetDto createTweetDto)
        {
            if (string.IsNullOrWhiteSpace(createTweetDto.Content))
            {
                return null;
            }

            // UserId must reference an existing user
            if (_userRepository.GetUserById(createTweetDto.UserId) is null)
            {
                return null;
            }

            var createdTweet = _tweetRepository.AddTweet(new Tweet(createTweetDto.UserId, createTweetDto.Content));
            return ToDto(createdTweet);
        }

        public TweetDto? UpdateTweet(Guid id, UpdateTweetDto updateTweetDto)
        {
            var tweet = _tweetRepository.GetTweetById(id);
            if (tweet is null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(updateTweetDto.Content))
            {
                return null;
            }

            tweet.Content = updateTweetDto.Content;
            return ToDto(_tweetRepository.UpdateTweet(tweet));
        }

        public bool DeleteTweet(Guid id)
        {
            var tweet = _tweetRepository.GetTweetById(id);
            return tweet is not null && _tweetRepository.DeleteTweet(tweet);
        }

        private static TweetDto ToDto(Tweet tweet)
        {
            return new TweetDto
            {
                Id = tweet.Id,
                UserId = tweet.UserId,
                Content = tweet.Content
            };
        }
    }
}
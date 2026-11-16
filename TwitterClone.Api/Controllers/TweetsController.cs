using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitterClone.Api.Auth;
using TwitterClone.Application.Dtos;
using TwitterClone.Application.Interfaces;

namespace TwitterClone.Api.Controllers
{
    // api/tweets
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // only logged-in users (valid JWT) can call these endpoints
    public class TweetsController : ControllerBase
    {
        private readonly ITweetService _tweetService;

        public TweetsController(ITweetService tweetService)
        {
            _tweetService = tweetService;
        }

        // GET /api/tweets?userId={userId}
        [HttpGet]
        [ProducesResponseType(typeof(List<TweetDto>), StatusCodes.Status200OK)]
        public IActionResult GetTweets([FromQuery] Guid? userId)
        {
            return Ok(_tweetService.GetTweets(userId));
        }

        // GET /api/tweets/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TweetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTweetById([FromRoute] Guid id)
        {
            var tweet = _tweetService.GetTweetById(id);

            if (tweet == null)
            {
                return NotFound();
            }

            return Ok(tweet);
        }

        // POST /api/tweets
        [HttpPost]
        [ProducesResponseType(typeof(TweetDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateTweet([FromBody] CreateTweetDto createTweetDto)
        {
            // The author is whoever owns the access token, never a userId sent in the body.
            var createdTweet = _tweetService.CreateTweet(User.GetUserId(), createTweetDto);

            if (createdTweet == null)
            {
                return BadRequest("Your account no longer exists.");
            }

            return CreatedAtAction(nameof(GetTweetById), new { id = createdTweet.Id }, createdTweet);
        }

        // PUT /api/tweets/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TweetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateTweet([FromRoute] Guid id, [FromBody] UpdateTweetDto updateTweetDto)
        {
            var tweet = _tweetService.GetTweetById(id);

            if (tweet == null)
            {
                return NotFound();
            }

            if (tweet.UserId != User.GetUserId())
            {
                // Logged in, but this tweet belongs to someone else.
                return Forbid();
            }

            return Ok(_tweetService.UpdateTweet(id, updateTweetDto));
        }

        // DELETE /api/tweets/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteTweet([FromRoute] Guid id)
        {
            var tweet = _tweetService.GetTweetById(id);

            if (tweet == null)
            {
                return NotFound();
            }

            if (tweet.UserId != User.GetUserId())
            {
                // Logged in, but this tweet belongs to someone else.
                return Forbid();
            }

            _tweetService.DeleteTweet(id);

            return NoContent();
        }
    }
}

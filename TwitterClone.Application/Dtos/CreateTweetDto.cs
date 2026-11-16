using System.ComponentModel.DataAnnotations;

namespace TwitterClone.Application.Dtos
{
    public class CreateTweetDto : IValidatableObject
    {
        [Required]
        public required string Content { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return TweetContentValidator.Validate(Content, nameof(Content));
        }
    }
}

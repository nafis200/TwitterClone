using System.ComponentModel.DataAnnotations;

namespace TwitterClone.Application.Dtos
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public required string RefreshToken { get; set; }
    }
}

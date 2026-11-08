using System.ComponentModel.DataAnnotations;

namespace TwitterClone.Application.Dtos
{
    public class ChangePasswordDto
    {
        [Required]
        public required string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public required string NewPassword { get; set; }
    }
}

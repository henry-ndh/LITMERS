using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request
{
    public class UpdateProfileRequestDTO
    {
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 50 characters")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Avatar URL must be less than 500 characters")]
        public string? Avatar { get; set; }
    }
}


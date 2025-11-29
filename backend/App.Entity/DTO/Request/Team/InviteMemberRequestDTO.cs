using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request.Team
{
    public class InviteMemberRequestDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; }
    }
}

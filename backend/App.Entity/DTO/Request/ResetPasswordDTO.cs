using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string NewPassword { get; set; }
    }
}

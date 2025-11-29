using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}

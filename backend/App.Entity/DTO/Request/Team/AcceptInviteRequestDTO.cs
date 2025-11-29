using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request.Team
{
    public class AcceptInviteRequestDTO
    {
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }
    }
}

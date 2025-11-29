using System.ComponentModel.DataAnnotations;
using App.Entity.Models.Enums;

namespace App.Entity.DTO.Request.Team
{
    public class UpdateMemberRoleRequestDTO
    {
        [Required(ErrorMessage = "Role is required")]
        public TeamRole Role { get; set; }
    }
}

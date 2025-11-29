using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request.Team
{
    public class UpdateTeamRequestDTO
    {
        [Required(ErrorMessage = "Team name is required")]
        [StringLength(50, ErrorMessage = "Team name cannot exceed 50 characters")]
        public string Name { get; set; }
    }
}

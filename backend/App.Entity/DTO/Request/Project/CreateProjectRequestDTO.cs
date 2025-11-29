using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request.Project
{
    public class CreateProjectRequestDTO
    {
        [Required(ErrorMessage = "Team ID is required")]
        public long TeamId { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters")]
        public string Name { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }
    }
}


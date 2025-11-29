using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request.Issue
{
    public class CreateIssueStatusRequestDTO
    {
        [Required(ErrorMessage = "Status name is required")]
        [StringLength(30, ErrorMessage = "Status name cannot exceed 30 characters")]
        public string Name { get; set; }

        [StringLength(7, ErrorMessage = "Color must be 7 characters (hex format)")]
        public string? Color { get; set; }

        [Required(ErrorMessage = "Position is required")]
        public int Position { get; set; }

        public bool IsDefault { get; set; } = false;

        public int? WipLimit { get; set; } // null = unlimited
    }
}


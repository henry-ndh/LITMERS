using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request.Issue
{
    public class CreateProjectLabelRequestDTO
    {
        [Required(ErrorMessage = "Label name is required")]
        [StringLength(30, ErrorMessage = "Label name cannot exceed 30 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Color is required")]
        [StringLength(7, ErrorMessage = "Color must be 7 characters (hex format)")]
        public string Color { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request.Issue
{
    public class CreateSubtaskRequestDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Position is required")]
        public int Position { get; set; }
    }
}


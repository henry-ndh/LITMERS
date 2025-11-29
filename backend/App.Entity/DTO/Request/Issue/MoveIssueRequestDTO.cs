using System.ComponentModel.DataAnnotations;

namespace App.Entity.DTO.Request.Issue
{
    public class MoveIssueRequestDTO
    {
        [Required(ErrorMessage = "Status ID is required")]
        public long StatusId { get; set; }

        [Required(ErrorMessage = "Position is required")]
        public int Position { get; set; }
    }
}


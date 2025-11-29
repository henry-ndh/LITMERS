using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Entity.Models.Enums;

namespace App.Entity.DTO.Request
{
    public class RegisterDTO
    {
        public string? Name { get; set; }

        [Required]
        [StringLength(250)]
        public string? Password{ get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }


    }
}

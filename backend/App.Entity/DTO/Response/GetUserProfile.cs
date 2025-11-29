using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Entity.Models.Enums;

namespace App.Entity.DTO.Response
{
    public class GetUserProfile
    {
        public long Id { get; set; }
        public string? Name { get; set; }

        public string? Email { get; set; }

        public Gender Gender { get; set; } = 0;

        public string? Password { get; set; } = null;

        public string? Avatar { get; set; }
        public bool IsActive { get; set; } = true;

        public UserRole Role { get; set; } = UserRole.USER;
        public string? GoogleId { get; set; } = string.Empty;
        public UserOrigin UserOrigin { get; set; } = UserOrigin.System;
    }
}

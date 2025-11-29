using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using App.Entity.Common;
using App.Entity.Models.Enums;

namespace App.Entity.Models
{
    [Table("users")]
    public class UserModel : BaseEntity
    {
        [Column("name")]
        public string? Name { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Column("gender")]
        public Gender Gender { get; set; } = 0;

        [Column("password")]
        public string? Password { get; set; } = null;

        [Column("avatar")]
        public string? Avatar { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("role")]
        public UserRole Role { get; set; } = UserRole.USER;

        [Column("google_id")]
        public string? GoogleId { get; set; } = string.Empty;

        [Column("user_origin")]
        public UserOrigin UserOrigin { get; set; } = UserOrigin.System;
    }
}

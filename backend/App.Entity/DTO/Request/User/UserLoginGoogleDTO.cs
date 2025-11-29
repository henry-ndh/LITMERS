using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Entity.DTO.Request.User
{
    public class UserLoginGoogleDTO
    {
        public string Name { get; set; }
        public string  Email { get; set; }
        public string GoogleId { get; set; }
        public string Avatar { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Entity.DTO.Request
{
    public class LoginDTO
    {
        public string EmailOrUserName { get; set; }

        public string Password { get; set; }
    }
}

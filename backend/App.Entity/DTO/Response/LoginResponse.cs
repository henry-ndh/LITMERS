using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Entity.DTO.Response
{
    public class LoginResponse
    {
        public string? Email {  get; set; }

        public string? UserName {  get; set; }

        public string? AccessToken {  get; set; }

        public string? RefeshToken { get; set; }

        public string? Message { get; set; }

    }
}

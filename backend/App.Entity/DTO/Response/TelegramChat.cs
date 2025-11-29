using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Entity.DTO.Response
{
    public class TelegramChat
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string Type { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Entity.DTO.Response
{
    public class TelegramUpdate
    {
        public long UpdateId { get; set; }
        public TelegramMessage Message { get; set; }
    }
}
    
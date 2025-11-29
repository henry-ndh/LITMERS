using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Entity.DTO.Response
{
    public class TelegramResponse
    {

            public bool Ok { get; set; }
            public List<TelegramUpdate> Result { get; set; }
        
    }
}

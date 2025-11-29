using System;
using System.Collections.Generic;
using System.Text;

namespace TFU.MessagePlatform.Zalo
{
   public class ZaloTokenResponse
   {
      public string access_token { get; set; }
      public int expires_in { get; set; }
   }
}

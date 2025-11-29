using System;
using System.Collections.Generic;
using System.Text;

namespace TFU.MessagePlatform.Zalo
{
   public class CallbackModelResponse
   {
      public string uid { get; set; }
      public string code { get; set; }
      public string scope { get; set; }
   }
}

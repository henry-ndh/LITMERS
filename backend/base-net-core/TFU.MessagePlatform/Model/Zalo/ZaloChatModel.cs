using System;
using System.Collections.Generic;
using System.Text;

namespace TFU.MessagePlatform.Zalo
{
   public class ZaloChatModel
   {

   }
   public class Recipient
   {
      public string user_id { get; set; }
   }
   public class Message
   {
      public Attachment attachment { get; set; }
      public string text { get; set; }
   }
   public class Attachment
   {
      public string type { get; set; }
      public PayLoad payload { get; set; }
   }
   public class PayLoad
   {
      public string token { get; set; }
   }
}

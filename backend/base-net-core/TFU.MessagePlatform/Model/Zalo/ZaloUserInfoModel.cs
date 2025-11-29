using System;
using System.Collections.Generic;
using System.Text;

namespace TFU.MessagePlatform.Zalo
{
  public  class ZaloUserInfoModel
   {
		public string id { get; set; }
		public string name { get; set; }
		public string gender { get; set; }
		public string birthday { get; set; }
		public object picture { get; set; }
	}
}

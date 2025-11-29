using System;
using System.Collections.Generic;
using System.Text;

namespace TFU.MessagePlatform.Zalo
{
	public class ZaloResponse<T>
	{
		public T data { get; set; }
		public int error { get; set; }
		public string message { get; set; }
		public bool Success { get { return message == "Success"; } }
	}
}

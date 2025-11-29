using System;
using System.Collections.Generic;
using System.Text;

namespace TFU.MessagePlatform.Zalo
{

	public class PostMessageModel
	{
		public Recipient recipient { get; set; }
		public Message message { get; set; }
	}
}

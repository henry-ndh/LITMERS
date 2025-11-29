using System;
using System.Collections.Generic;
using System.Text;

namespace TFU.MessagePlatform.Zalo
{
	public class ZaloWebhookMessage
	{
		public string app_id { get; set; }
		public Sender sender { get; set; }
		public string user_id_by_app { get; set; }
		public Recipient recipient { get; set; }
		public string event_name { get; set; }
		public Message message { get; set; }
		public string timestamp { get; set; }
	}

	public class Sender
	{
		public string id { get; set; }
	}
}


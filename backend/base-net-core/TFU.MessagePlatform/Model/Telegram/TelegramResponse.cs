namespace TFU.MessagePlatform.Model.Telegram
{
	public class TelegramResponse
	{
		public bool ok { get; set; }
		public Result result { get; set; }
		public bool Success => ok;
	}

	public class Result
	{
		public long message_id { get; set; }
		public From from { get; set; }
		public Chat chat { get; set; }
		public long date { get; set; }
		public string text { get; set; }
	}

	public class From
	{
		public long id { get; set; }
		public bool is_bot { get; set; }
		public string first_name { get; set; }
		public string username { get; set; }
	}

	public class Chat
	{
		public long id { get; set; }
		public string first_name { get; set; }
		public string last_name { get; set; }
		public string type { get; set; }
	}

}

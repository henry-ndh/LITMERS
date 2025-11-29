namespace TFU.MessagePlatform.Model.Telegram
{
	public class TelegramPostModel
	{
		public long chat_id { get; set; }
		public string text { get; set; }
		public InlineKeyboardMarkup reply_markup { get; set; }
		public TelegramPostModel()
		{

		}
	}

	public class TelegramRanusModel
	{
		public string CreatedBy { get; set; }
		public string Text { get; set; }
		public TelegramRanusModel()
		{

		}
	}

	public class WebApp
	{
		public string url { get; set; }
	}

	public class InlineKeyboardButton
	{
		public string text { get; set; }
		public WebApp web_app { get; set; }
	}

	public class InlineKeyboardMarkup
	{
		public InlineKeyboardButton[][] inline_keyboard { get; set; }
	}







}

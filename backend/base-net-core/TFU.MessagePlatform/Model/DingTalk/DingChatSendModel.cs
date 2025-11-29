namespace App.Entity.Models.DingTalk
{
	public class DingChatSendModel
	{
		public string chatid { get; set; }
		public string msgtype { get; set; } = "text";
		public Text text { get; set; } = new Text() { };
	}

	public class Text
	{
		public string content { get; set; }
	}
}

namespace App.Entity.Models.DingTalk
{
	public class DingChatCreateBodyModel
	{
		//Task Id của hệ thống EP
		public long taskId { get; set; }
		public string name { get; set; }
		public string owner { get; set; }
		public string[] useridlist { get; set; }
	}
}

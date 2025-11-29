namespace TFU.Common.Models
{
	public class RefreshPasswordModel
	{
		public string Password { get; set; }
		public string NewPassword { get; set; }
		public string ConfirmPassword { get; set; }
		public string Token { get; set; }
		public string UserId { get; set; }
	}
}

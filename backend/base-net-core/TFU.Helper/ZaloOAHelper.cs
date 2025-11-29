using RestSharp;
using System.Threading.Tasks;

namespace TFU.Helper
{
	public static class ZaloOAHelper
	{
		public static string ApiUrl { get; set; }
		public const string CLIENT = "ranus_application";
		/// <summary>
		/// Send notification via Zalo OA.
		/// </summary>
		/// <param name="userEmails"></param>
		/// <param name="message"></param>
		public static async Task SendAffiliateNotification(string[] phoneNumbers, string message)
		{
			var client = new RestClient(ApiUrl);
			var request = new RestRequest("/api/zalooa/notify-to-users");
			request.Method = Method.Post;
			request.AddHeader("client", CLIENT);
			request.AddHeader("Content-Type", "application/json");
			request.AddJsonBody(new
			{
				phoneNumbers,
				message
			});
			await client.ExecuteAsync(request);
		}
	}
}

using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace TFU.Pixel
{
	public class PixelHelper
	{
		public static async Task<object> MetaCallAPI(string api, string access_token, dynamic? queryParams = null, dynamic? bodyParams = null, Method method = Method.Get)
		{
			RestClient client = new RestClient("https://graph.facebook.com/v15.0");
			RestRequest request = new RestRequest(api, method);
			request.AddQueryParameter("access_token", access_token);

			//query params 
			if (queryParams != null)
			{
				var d = queryParams as Dictionary<string, dynamic>;
				foreach (var item in d)
				{
					request.AddQueryParameter(item.Key, $"{item.Value}");
				}
			}

			//body params
			if (bodyParams != null)
			{
				string jsonStr = JsonConvert.SerializeObject(bodyParams, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
				request.AddJsonBody(jsonStr);
			}

			var response = await client.ExecuteAsync(request);
			if (response != null && response.StatusCode == HttpStatusCode.OK)
			{
				return JsonConvert.DeserializeObject(response.Content);
			}
			else
			{
				return null;
			}
		}

		public static string GetSHA256(string input)
		{
			using (var sha256 = SHA256.Create())
			{
				byte[] inputBytes = Encoding.UTF8.GetBytes(input);
				byte[] hashBytes = sha256.ComputeHash(inputBytes);
				return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
			}
		}
	}
}

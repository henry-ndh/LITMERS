using RestSharp;
using TFU.Pixel.Model;

namespace TFU.Pixel
{
	public static class MetaPixel
	{
		private const string MetaConversionToken = "EAAKr3sjLAQ8BOZCrOZAxjVOSiM3TEjJGOyRHYqxb80syYqu4BXljcb3T7eYoRko5ZAd1v5tUAfXkUZC0Pa1C1uzAabKUG9akTKYsNEraXkPJK94ZBSsnwY9hCnxLOeUqYCke1iOxIfJn5PZCZAJLFqHgYU3CZBnRZANEN68HDKf15uIum3IniLhRX4LxBZCCBmelhaOQZDZD";
		private const string MetaMarketingToken = "EAAJcYKrEI7gBOw4OwVDC6PYS0Mru2Wq6nrSqoZAtOO2HEwnClVRM5QdkmnjhaKsyfgEhmDxlwrSEJ1e5SsNtoAkSl81vetXX83Ip6ZAsSDGHig0Sxz8wuKbjKJAtL21lDpvf0HWVpU4CWLbX4wKGc2AIXyMN1PLI87M8WxwDrKVLZCKY3eZB7ewpmrH4USn17j0fxrQN";
		private const string PixelId = "6997923986972179";
		private const string CatalogId = "1060222788612040";
		public const string GoToWebEvent = "GoToWeb";
		public const string BotUserEvent = "BotUser";

		public static async Task<object> PushEventToMetaPixel(MetaPixelModel model)
		{
			var api = $"/{PixelId}/events";
			var data = await PixelHelper.MetaCallAPI(api, MetaConversionToken, null, model, Method.Post);
			return data;
		}

		public static async Task<object> PushProductToMetaPixel(ProductCatalogModel model)
		{
			Dictionary<string, dynamic> keyValuePairs = new Dictionary<string, dynamic>();
			var properties = model.GetType().GetProperties();
			foreach (var item in properties)
			{
				object value = item.GetValue(model);
				keyValuePairs.Add(item.Name, value);
			}
			var api = $"/{CatalogId}/products";
			var data = await PixelHelper.MetaCallAPI(api, MetaMarketingToken, keyValuePairs, null, Method.Post);
			return data;
		}
	}
}
namespace TFU.Pixel.Model
{
	public class ProductCatalogModel
	{
		public string name { get; set; }
		public string description { get; set; }
		public string url { get; set; }
		public string image_url { get; set; }
		public string brand { get; set; }
		public int price { get; set; }
		public int sale_price { get; set; }
		public string currency { get; set; }
		public string availability { get; set; }
		public string condition { get; set; }
		public int retailer_id { get; set; }
	}
}

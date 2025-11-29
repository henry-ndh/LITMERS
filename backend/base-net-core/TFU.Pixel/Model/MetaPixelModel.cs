namespace TFU.Pixel.Model
{
	public class MetaPixelModel
	{
		public MetaDataModel[] data { get; set; }
	}

	public class MetaDataModel
	{
		public string event_name { get; set; }
		public int event_time { get; set; }
		public string action_source { get; set; }
		public User_Data user_data { get; set; }
		public Custom_Data custom_data { get; set; }
		public MetaDataModel()
		{

		}
	}

	public class User_Data
	{
		public string[] em { get; set; }
		public string[] ph { get; set; }
		public string client_user_agent { get; set; }
		public string client_ip_address { get; set; }
		public string fbc { get; set; }
		public string fbp { get; set; }
		public int fb_login_id { get; set; }
		public string[] external_id { get; set; }
		public User_Data()
		{

		}
	}

	public class Custom_Data
	{
		public string content_category { get; set; }
	}

}

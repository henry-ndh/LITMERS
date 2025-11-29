using System;
using System.Collections.Generic;
using System.Text;

namespace TFU.Common.Models
{
	public class FacebookUserModel
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public FacebookPictureModel Picture { get; set; }
		public string Email { get; set; }
	}
	public class FacebookPictureModel
	{
		public FacebookPictureDataModel Data { get; set; }
	}

	public class FacebookPictureDataModel
	{
		public long Height { get; set; }
		public long Width { get; set; }
		public string Url { get; set; }
	} 
}

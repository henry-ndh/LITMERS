using System;
using System.Collections.Generic;
using System.Text;

namespace App.Entity.Models.DingTalk
{
	public class DingTokenModel
	{
		public int errcode { get; set; }
		public string access_token { get; set; }
		public string errmsg { get; set; }
		public int expires_in { get; set; }
	}
}

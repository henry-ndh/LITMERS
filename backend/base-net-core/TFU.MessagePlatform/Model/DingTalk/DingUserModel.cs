using System;
using System.Collections.Generic;
using System.Text;

namespace App.Entity.Models.DingTalk
{
	public class DingUserModel
	{
		public string unionid { get; set; }
		public string openId { get; set; }
		public bool isLeader { get; set; }
		public string mobile { get; set; }
		public bool active { get; set; }
		public bool isAdmin { get; set; }
		public string avatar { get; set; }
		public string userid { get; set; }
		public bool isHide { get; set; }
		public bool isBoss { get; set; }
		public string name { get; set; }
		public string stateCode { get; set; }
		public int[] department { get; set; }
		public long order { get; set; }
		public string position { get; set; }
		public string remark { get; set; }
		public string jobnumber { get; set; }
		public string tel { get; set; }
		public string workPlace { get; set; }
		public string email { get; set; }
	}
}

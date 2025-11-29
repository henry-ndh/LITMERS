using System;
using System.Collections.Generic;
using System.Text;

namespace App.Entity.Models.DingTalk
{
	public class DingDepartmentModel
	{
		public bool createDeptGroup { get; set; }
		public string name { get; set; }
		public int id { get; set; }
		public bool autoAddUser { get; set; }
		public int parentid { get; set; }
	}
}

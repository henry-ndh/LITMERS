using System;
using System.Collections.Generic;
using System.Text;

namespace TFU.Common.Models
{
	public class Rootobject
	{
		public Datum[] data { get; set; }
		public Paging paging { get; set; }
	}

	public class Paging
	{
		public Cursors cursors { get; set; }
	}

	public class Cursors
	{
		public string before { get; set; }
		public string after { get; set; }
	}

	public class Datum
	{
		public string name { get; set; }
		public string id { get; set; }
		public DateTime created_time { get; set; }
	}


}

using System;
using System.Collections.Generic;
using System.Text;

namespace App.Utility
{
	public class DingTalkResponse<T>
	{
		public int Errcode { get; set; }
		public string Errmsg { get; set; }
		public string Request_id { get; set; }
		public bool Success => Errcode == 0;

		public object Result { get; set; }

		#region Department
		public T[] Department { get; set; }
		#endregion

		#region User
		public T[] Userlist { get; set; }
		public bool HasMore { get; set; }
      #endregion

      #region Chat
		public T ChatId { get; set; }
		public T MessageId { get; set; }
		#endregion

	}
}

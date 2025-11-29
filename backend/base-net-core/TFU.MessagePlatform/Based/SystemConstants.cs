using System;
using System.Collections.Generic;
using System.Text;

namespace App.Utility
{
	public class SystemConstants
	{
		#region Google API
		public const string DriveV2Files = "https://www.googleapis.com/drive/v2/files/";
		#endregion

		#region Dingtalk API
		//Custom Keyword Security
		public const string DingCustomKey = "Notice:";

		//Credential
		public const string DingGetToken = "/ gettoken";

		//AddressBook
		public const string DingUserListByPage = "/user/listbypage";
		public const string DingDepartmentList = "/department/list";
		public const string DingCreateChat = "/chat/create";
		public const string DingSendChat = "/chat/send";
		public const string DingRobotSendChat = "/robot/send";

		#endregion
	}
}

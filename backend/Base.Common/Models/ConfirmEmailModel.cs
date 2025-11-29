using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TFU.Common.Models
{
	public class ConfirmEmailModel
	{
		public string UserId { get; set; }
		public string Token { get; set; }
	}
}

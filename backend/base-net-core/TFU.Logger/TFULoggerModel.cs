using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace TFU.Logger
{
	public class TFULoggerModel
	{
		[Key]
		public long Id { get; set; }
		public LogLevel LogLevel { get; set; }
		public int EventId { get; set; }
		public string CategoryName { get; set; }
		public string Msg { get; set; }
		public DateTime Time { get; set; }

		public override string ToString()
		{
			var style = "color:black;";
			switch (LogLevel)
			{
				case LogLevel.Information:
					style = "color:blue;";
					break;
				case LogLevel.Warning:
					style = "color:orange;";
					break;
				case LogLevel.Error:
					style = "color:red;";
					break;
				default:
					break;
			}
			return $"<p><strong style='{style}'>{CategoryName}</strong> ({Time}):<br/><span>{Msg}</span><p><hr>";
		}
	}
}

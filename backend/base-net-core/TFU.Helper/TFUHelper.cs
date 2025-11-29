using System.Drawing;
using System.Xml.Serialization;

namespace TFU.Helper
{
	public class TFUHelper
	{
		public static StringFormat ConvertTextAlignToStringFormat(dynamic textAlignDynamic)
		{
			return new StringFormat(StringFormat.GenericTypographic)
			{
				Alignment = StringAlignment.Near,
				LineAlignment = StringAlignment.Center
			};
		}
		public enum TextAlign
		{
			[XmlEnum("1")]
			Left = 1,
			[XmlEnum("2")]
			Center = 2,
			[XmlEnum("3")]
			Right = 3,
			[XmlEnum("4")]
			Justify = 4,
		}
	}
}

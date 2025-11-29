using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TFU.Common.Extension;

namespace TFU.MVCWebBased
{
	public class MobileViewLocationExpander : IViewLocationExpander
	{
		public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
		{
			var results = new List<string>();
			bool isMobile = context.Values["Device"] == "Mobile";
			foreach (var item in viewLocations)
			{
				var result = isMobile ? item.Replace("/Views/", "/MobileViews/") : item;
				results.Add(result);
			}
			return results;
		}

		public void PopulateValues(ViewLocationExpanderContext context)
		{
			bool isMobile = context.ActionContext.HttpContext.IsMobileDevice();
			context.Values["Device"] = isMobile ? "Mobile" : "Desktop";
		}
	}
}

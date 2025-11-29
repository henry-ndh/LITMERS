using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TFU.WebBased
{
	public static class WebUtils
	{
		public static async Task<string> RenderViewAsync<TModel>(Controller controller, string viewName, TModel model, bool partial = false)
		{
			if (string.IsNullOrEmpty(viewName))
			{
				viewName = controller.ControllerContext.ActionDescriptor.ActionName;
			}

			controller.ViewData.Model = model;

			using (var writer = new StringWriter())
			{
				IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
				ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

				if (viewResult.Success == false)
				{
					return $"A view with the name {viewName} could not be found";
				}

				ViewContext viewContext = new ViewContext(
					 controller.ControllerContext,
					 viewResult.View,
					 controller.ViewData,
					 controller.TempData,
					 writer,
					 new HtmlHelperOptions()
				);

				await viewResult.View.RenderAsync(viewContext);

				return writer.GetStringBuilder().ToString();
			}
		}

		public static string FormatPrice(long? price)
		{
			CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
			var format = string.Format(elGR, "{0:0,0}", price);
			return price?.ToString("0,0", elGR) ?? "0";// String.Format("{#.##0}", price);
		}
	}
}

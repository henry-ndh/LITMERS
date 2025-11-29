using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Base.Common.Attributes
{
    public class RequireEventCodeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var hasHeader = context.HttpContext.Request.Headers.TryGetValue("X-Event-Code", out var eventCode);
            if (!hasHeader || string.IsNullOrWhiteSpace(eventCode))
            {
                context.Result = new BadRequestObjectResult("Missing or empty Event");
            }
        }
    }

}

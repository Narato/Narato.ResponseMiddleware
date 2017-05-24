using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Narato.ResponseMiddleware.Models.Legacy.Models;
using System;
using System.Diagnostics;

namespace Narato.ResponseMiddleware.ResponseFilters
{
    public class LegacyExecutionTimingFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var sw = new Stopwatch();
            sw.Start();
            context.HttpContext.Items.Add("api_timing", sw);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

            var sw = context.HttpContext.Items["api_timing"] as Stopwatch;

            if (sw != null)
            {
                sw.Stop();
                var objectResult = context.Result as ObjectResult;

                if (objectResult != null && objectResult.Value is Response)
                {
                    (objectResult.Value as Response).Generation.Duration = sw.ElapsedMilliseconds;
                    (objectResult.Value as Response).Generation.TimeStamp = DateTime.Now;
                }
            }
        }
    }
}

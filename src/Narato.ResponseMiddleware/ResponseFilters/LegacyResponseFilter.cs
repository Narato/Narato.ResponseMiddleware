using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Narato.ResponseMiddleware.Models.Legacy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Narato.ResponseMiddleware.ResponseFilters
{
    public class LegacyResponseFilter : IActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var bla = 5;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // let exceptionhandlerfilter handle this
            if (context.Exception != null)
                return;

            if (context.Result != null && context.Result is ObjectResult)
            {
                var objectResult = context.Result as ObjectResult;
                objectResult.Value = new Response<object>(objectResult.Value, context.HttpContext.Request.Path, objectResult.StatusCode ?? 200);
                //var response = new Response<object>(objectResult.Value, context.HttpContext.Request.Path, objectResult.StatusCode ?? 200);
                //context.Result = new ObjectResult(response) { StatusCode = objectResult.StatusCode };
            }
        }
    }
}

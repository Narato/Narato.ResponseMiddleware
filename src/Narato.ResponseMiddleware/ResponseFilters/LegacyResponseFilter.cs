using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Narato.ResponseMiddleware.Models.Legacy.Models;
using Narato.ResponseMiddleware.Models.Models.Interfaces;

namespace Narato.ResponseMiddleware.ResponseFilters
{
    public class LegacyResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // do nothing
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // let exceptionhandlerfilter handle this
            if (context.Exception != null)
                return;
            if (context.Result == null)
                return;

            if (context.Result is ObjectResult)
            {
                var objectResult = context.Result as ObjectResult;
                
                if (objectResult.Value is IPaged<object>)
                {
                    var pagedValue = objectResult.Value as IPaged<object>;

                    var response = new Response<object>(pagedValue.Items, context.HttpContext.Request.Path, objectResult.StatusCode ?? 200);
                    response.Skip = pagedValue.Skip;
                    response.Take = pagedValue.Take;
                    response.Total = pagedValue.Total;
                    objectResult.Value = response;
                } else
                {
                    objectResult.Value = new Response<object>(objectResult.Value, context.HttpContext.Request.Path, objectResult.StatusCode ?? 200);
                    objectResult.DeclaredType = objectResult.Value.GetType(); // needed in case the controller endpoint returns a string, we'd use the StringOutputFormatter, which crashes if it tries to write a response object
                }
            } else if (context.Result is StatusCodeResult)
            {
                // we have to convert this to an ObjectResult
                var statusCodeResult = context.Result as StatusCodeResult;

                var objectResult = new ObjectResult(new Response
                {
                    Status = statusCodeResult.StatusCode,
                    Self = context.HttpContext.Request.Path
                });
                objectResult.StatusCode = statusCodeResult.StatusCode;
                context.Result = objectResult;
            }
        }
    }
}

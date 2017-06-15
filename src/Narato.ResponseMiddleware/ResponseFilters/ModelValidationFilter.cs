using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Narato.ResponseMiddleware.Models.Models;

namespace Narato.ResponseMiddleware.ResponseFilters
{
    public class ModelValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var modelValidationDictionary = GetModelValidationDictionary(context.ModelState);

                // null to bypass the typed constructor
                context.Result = new BadRequestObjectResult(new ValidationErrorContent<string>(null) { ValidationMessages = modelValidationDictionary });
            }
        }

        private ModelValidationDictionary<string> GetModelValidationDictionary(ModelStateDictionary modelState)
        {
            var modelValidationDictionary = new ModelValidationDictionary<string>();
            foreach (var modelstateItem in modelState)
            {
                foreach (var error in modelstateItem.Value.Errors)
                {
                    var errorMessage = error.ErrorMessage;
                    if (string.IsNullOrEmpty(errorMessage))
                        errorMessage = error.Exception.Message;
                    modelValidationDictionary.Add(modelstateItem.Key, errorMessage);
                }
            }

            return modelValidationDictionary;
        }
    }
}

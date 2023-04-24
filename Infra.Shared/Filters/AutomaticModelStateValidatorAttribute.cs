using System.Linq;
using Infra.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Infra.Shared.Filters
{
    public class AutomaticModelStateValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            if (actionExecutingContext.ModelState.IsValid)
                return;

            var errors = actionExecutingContext.ModelState.ToDictionary(modelState => modelState.Key, modelState =>
                modelState.Value.Errors.Select(a => a.ErrorMessage).ToList());

            var badRequestException = new BadRequestException("One or more validation errors occurred.", errors);

            actionExecutingContext.Result = new BadRequestObjectResult(badRequestException);
        }
    }
}
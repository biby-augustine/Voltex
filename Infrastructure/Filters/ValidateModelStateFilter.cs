using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Infrastructure.Filters
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            var validationErrors = context.ModelState
                .Keys
                .SelectMany(k => context.ModelState[k].Errors)
                .Select(e => e.ErrorMessage);

            context.Result = new BadRequestObjectResult(new ErrorResponse
            {
                Messages = validationErrors
            });
        }
    }
}

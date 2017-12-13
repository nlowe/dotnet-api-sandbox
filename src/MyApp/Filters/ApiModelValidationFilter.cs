using Microsoft.AspNetCore.Mvc.Filters;
using MyApp.Exceptions;

namespace MyApp.Filters
{
    public class ApiModelValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                throw new BadModelException(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
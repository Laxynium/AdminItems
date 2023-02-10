using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdminItems.Api.AdminItems;

public class OptimisticConcurrencyExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is OptimisticConcurrencyException exception)
        {
            context.Result = new ConflictObjectResult(new { });

            context.ExceptionHandled = true;
        }
    }
}
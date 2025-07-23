using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Extensions;

namespace WebApi.Filters;

public class ValidAppRequestContextFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var requestContext = context.HttpContext.Request.GetAppRequestContext();

        if (requestContext.UserId <= 0)
        {
            context.Result = new UnauthorizedResult();
        }

        base.OnActionExecuting(context);
    }
}
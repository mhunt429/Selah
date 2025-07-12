using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters;

public class DebugOnlyAsyncActionFilter : Attribute, IAsyncActionFilter
{
    private readonly IConfiguration _configuration;

    public DebugOnlyAsyncActionFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (_configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") != "Development")
        {
            context.Result = new NotFoundResult();
            return;
        }

        await next();
    }
}
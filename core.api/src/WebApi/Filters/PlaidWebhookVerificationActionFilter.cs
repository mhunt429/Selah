using System.IdentityModel.Tokens.Jwt;
using Domain.Models;
using Domain.Models.Plaid;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters;

public class PlaidWebhookVerificationActionFilter(IPlaidHttpService plaidHttpService, IWebHostEnvironment env) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        if (env.EnvironmentName == "IntegrationTests")
        {
            await next();
            return;
        }
        
        var headers = context.HttpContext.Request.Headers;

        if (!headers.TryGetValue("Plaid-Verification", out var plaidHeader))
        {
            context.Result = new BadRequestObjectResult("Missing Plaid-Verification header");
        }

        var jwt = plaidHeader.FirstOrDefault();
        if (string.IsNullOrEmpty(jwt) || !await IsValidJwt(jwt))
        {
            context.Result = new BadRequestObjectResult("Invalid Plaid-Verification token");
            return; 
        }

        await next();
    }

    //TODO validate expiration timestamps
    private async Task<bool> IsValidJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        if (token == null)
        {
            return false;
        }

        string kidHeader = token.Header.Kid;

        string alg = token.Header.Alg;

        if (string.IsNullOrEmpty(kidHeader) || string.IsNullOrEmpty(alg) || alg != "ES256")
        {
            return false;
        }

        ApiResponseResult<PlaidWebhookVerificationResponse> verificationResult =
            await plaidHttpService.ValidateWebhook(kidHeader);

        if (verificationResult.status == ResultStatus.Success)
        {
            return true;
        }

        return false;
    }
}
using System.IdentityModel.Tokens.Jwt;
using Domain.Models;
using Domain.Models.Plaid;
using Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters;

public class PlaidWebhookVerificationActionFilter : Attribute, IAsyncActionFilter
{
    private readonly IPlaidHttpService _plaidHttpService;

    public PlaidWebhookVerificationActionFilter(IPlaidHttpService plaidHttpService)
    {
        _plaidHttpService = plaidHttpService;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var headers = context.HttpContext.Request.Headers;

        if (!headers.TryGetValue("Plaid-Verification", out var plaidHeader))
        {
            context.Result = new BadRequestObjectResult("Missing Plaid-Verification header");
            return; // ✅ STOP pipeline
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
            await _plaidHttpService.ValidateWebhook(kidHeader);

        if (verificationResult.status == ResultStatus.Success)
        {
            return true;
        }

        return false;
    }
}
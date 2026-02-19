using System.Net;
using Application.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Extensions;
using WebApi.Filters;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[ValidAppRequestContextFilter]
[EnableRateLimiting(Constants.UserTokenPolicy)]
[Route("api/[controller]")]
public class BankingController(BankingService bankingService) : ControllerBase
{
    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccountsQuery()
    {
        var appRequestContext = Request.GetAppRequestContext();

        var result = await bankingService.GetAccountsByUserId(appRequestContext.UserId);

        return Ok(result.ToBaseHttpResponse(HttpStatusCode.OK));
    }
}
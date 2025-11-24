using System.Net;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Filters;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[ValidAppRequestContextFilter]
[Route("api/[controller]")]
public class BankingController : ControllerBase
{
    private readonly BankingService _bankingService;

    public BankingController(BankingService bankingService)
    {
        _bankingService = bankingService;
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var appRequestContext = Request.GetAppRequestContext();

        var result = await _bankingService.GetAccountsByUserId(appRequestContext.UserId);

        return Ok(result.ToBaseHttpResponse(HttpStatusCode.OK));
    }
}
using System.Net;
using Application.Services;
using Domain.Constants;
using Domain.Models;
using Domain.Models.Plaid;
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
public class ConnectorController : ControllerBase
{
    private readonly ConnectorService _connectorService;

    public ConnectorController(ConnectorService connectorService)
    {
        _connectorService = connectorService;
    }

    [HttpGet("link")]
    public async Task<IActionResult> GetLinkToken()
    {
        // The ValidAppRequestContextFilter handles the case where request context is null
        var requestContext = Request.GetAppRequestContext();
        var userId = requestContext.UserId;

        var result = await _connectorService.GetLinkToken(userId);

        if (result.status != ResultStatus.Success) return BadRequest();

        return Ok(result.data.ToBaseHttpResponse(HttpStatusCode.OK));
    }


    [HttpPost("exchange")]
    public async Task<IActionResult> ExchangeToken([FromBody] TokenExchangeHttpRequest request)
    {
        var requestContext = Request.GetAppRequestContext();
        var userId = requestContext.UserId;

        request.UserId = userId;

        //TODO add error handling with useful API responses
        await _connectorService.ExchangeToken(request);

        return NoContent();
    }
}
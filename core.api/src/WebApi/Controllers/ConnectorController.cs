using System.Net;
using Application.Services;
using Domain.ApiContracts.Connector;
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
    public async Task<IActionResult> GetLinkTokenQuery()
    {
        // The ValidAppRequestContextFilter handles the case where request context is null
        var requestContext = Request.GetAppRequestContext();
        var userId = requestContext.UserId;

        var result = await _connectorService.GetLinkToken(userId);

        if (result.status != ResultStatus.Success) return BadRequest();

        return Ok(result.data.ToBaseHttpResponse(HttpStatusCode.OK));
    }


    [HttpPost("exchange")]
    public async Task<IActionResult> ExchangeTokenCommand([FromBody] TokenExchangeHttpRequest request)
    {
        var requestContext = Request.GetAppRequestContext();
        var userId = requestContext.UserId;

        request.UserId = userId;

        //TODO add error handling with useful API responses
        await _connectorService.ExchangeToken(request);

        return NoContent();
    }

    [HttpGet("update/{id}")]
    public async Task<IActionResult> UpdateConnectionQuery(int id)
    {
        var requestContext = Request.GetAppRequestContext();
        var userId = requestContext.UserId;
        var result = await _connectorService.GetLinkToken(userId, connectorId: id, forUpdate: true);

        if (result.status != ResultStatus.Success) return BadRequest();

        return Ok(result.data.ToBaseHttpResponse(HttpStatusCode.OK));
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateConnectionCommand(int id)
    {
        var requestContext = Request.GetAppRequestContext();
        var userId = requestContext.UserId;

        bool success = await _connectorService.UpdateConnection(id, userId);

        if (success)
        {
            return NoContent();
        }

        return BadRequest($"Unable to update connection for connectorId {id}");
    }
}
using System.Net;
using Application.AccountConnector;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;
using WebApi.Filters;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[ValidAppRequestContextFilter]
[Route("api/[controller]")]
public class ConnectorController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConnectorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("link")]
    public async Task<IActionResult> GetLinkToken()
    {
        var requestContext = Request.GetAppRequestContext();
        var userId = requestContext.UserId;

        var result = await _mediator.Send(new CreateLinkToken.Command
            { UserId = userId, AppRequestContext = requestContext });

        if (result.status != ResultStatus.Success) return BadRequest();

        return Ok(result.data.ToBaseHttpResponse(HttpStatusCode.OK));
    }

    public async Task<IActionResult> ExchangeToken([FromBody] ExchangeLinkToken.Command request)
    {
        var requestContext = Request.GetAppRequestContext();
        var userId = requestContext.UserId;

        request.UserId = userId;

        var result = await _mediator.Send(request);

        if (result.status != ResultStatus.Success) return BadRequest();

        return NoContent();
    }
}
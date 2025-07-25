using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.AccountConnector;
using Domain.Models;
using Domain.Models.Plaid;
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
        AppRequestContext requestContext = Request.GetAppRequestContext();
        int userId = requestContext.UserId;

        ApiResponseResult<PlaidLinkToken>
            result = await _mediator.Send(new CreateLinkToken.Command { UserId = userId });

        if (result.status != ResultStatus.Success)
        {
            return BadRequest();
        }

        return Ok(result.data.ToBaseHttpResponse(HttpStatusCode.OK));
    }

    [HttpPost("exchange")]
    public async Task<IActionResult> ExchangeToken([FromBody] ExchangeLinkToken.Command request)
    {
        AppRequestContext requestContext = Request.GetAppRequestContext();
        int userId = requestContext.UserId;

        request.UserId = userId;

        ApiResponseResult<Unit> result = await _mediator.Send(request);

        if (result.status != ResultStatus.Success)
        {
            return BadRequest();
        }

        return NoContent();
    }
}
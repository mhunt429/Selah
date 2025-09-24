using System.Net;
using Application.Banking;
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
public class BankingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BankingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var appRequestContext = Request.GetAppRequestContext();

        var query = new GetAccountsByUserId.Query { UserId = appRequestContext.UserId };
        var result = await _mediator.Send(query);

        return Ok(result.ToBaseHttpResponse(HttpStatusCode.OK));
    }
}
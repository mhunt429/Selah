using System.Net;
using Application.Services;
using Domain.ApiContracts.Mailbox;
using Domain.Constants;
using Domain.Models.Entities.Mailbox;
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
public class MailboxController(UserMailboxService mailboxService) : ControllerBase
{
    [HttpGet("messages")]
    public async Task<IActionResult> GetMessages()
    {
        var appRequestContext = Request.GetAppRequestContext();
        IEnumerable<MailboxResponse> result = await mailboxService.GetMessagesByUserId(appRequestContext.UserId);
        return Ok(result.ToBaseHttpResponse(HttpStatusCode.OK));
    }


    [HttpGet("messages/{id}")]
    public async Task<IActionResult> GetMessageById(int id)
    {
        var appRequestContext = Request.GetAppRequestContext();

        MailboxResponse? result = await mailboxService.GetMessagesByIdAndUserId(id, appRequestContext.UserId);

        return Ok(result.ToBaseHttpResponse(HttpStatusCode.OK));
    }

    [HttpDelete("messages/{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var appRequestContext = Request.GetAppRequestContext();

        await mailboxService.DeleteMessage(id, appRequestContext.UserId);

        return Ok($"Message Id {id} has been deleted");
    }

    [HttpDelete("messages")]
    public async Task<IActionResult> DeleteMessages()
    {
        var appRequestContext = Request.GetAppRequestContext();

        await mailboxService.DeleteAllMessages(appRequestContext.UserId);
        return Ok($"All messages have been deleted");
    }

    [HttpPut("messages/{id}")]
    public async Task<IActionResult> MarkAsRead([FromBody] MailboxUpdateRequest request, int id)
    {
        var appRequestContext = Request.GetAppRequestContext();

        await mailboxService.MarkMessageAsRead(id, appRequestContext.UserId);

        return NoContent();
    }
}
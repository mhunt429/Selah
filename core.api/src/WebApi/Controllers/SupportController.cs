using Application.Services;
using Domain.Constants;
using Domain.Models;
using Infrastructure;
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
public class SupportController(SupportService supportService) : ControllerBase
{
    [HttpGet("decrypted-connector-keys")]
    public async Task<IActionResult> GetDecryptedConnectorKeys()
    {
        AppRequestContext requestContext = Request.GetAppRequestContext();
        var keys = await supportService.GetDecryptedConnectorKeys(requestContext!.UserId);
        return Ok(keys);
    }
}
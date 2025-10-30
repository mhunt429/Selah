using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.ApplicationUser;
using Application.Identity;
using Domain.Models;
using Domain.ApiContracts;
using Domain.ApiContracts.Identity;
using WebApi.Extensions;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IMediator _mediatr;

    public IdentityController(IMediator mediatr)
    {
        _mediatr = mediatr;
    }

    /// <summary>
    /// Endpoint to get the authenticated user by subject JWT claim
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        AppRequestContext? requestContext = Request.GetAppRequestContext();

        int userId = requestContext.UserId;

        var query = new GetUserById.Query { UserId = userId };
        ApplicationUser? result = await _mediatr.Send(query);
        if (result == null)
        {
            return Unauthorized();
        }

        return Ok(result.ToBaseHttpResponse(HttpStatusCode.OK));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLogin.Command request)
    {
        UserLogin.Response? result = await _mediatr.Send(request);

        if (result == null || result.AccessToken == null)
        {
            return Unauthorized();
        }

        if (request.RememberMe)
        {
            Response.Cookies.Append("x_session_id", result.SessionId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
        }

        Response.Cookies.Append("x_api_token", result.AccessToken.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(30)
        });
        return Ok(result.AccessToken.ToBaseHttpResponse(HttpStatusCode.OK));
    }

    [AllowAnonymous]
    [HttpGet("current-session")]
    public async Task<IActionResult> GetCurrentSession()
    {
        var sessionIdHeader = Request.Cookies.FirstOrDefault(x => x.Key == "x_session_id");
        if (sessionIdHeader.Value == null)
        {
            return Unauthorized();
        }

        var sessionId = Guid.Empty;
        if (Guid.TryParse(sessionIdHeader.Value, out sessionId))
        {
            var query = new UserBySessionIdQuery.Query { SessionId = sessionId };

            var result = await _mediatr.Send(query);
            if (result != null)
            {
                return Ok(result.ToBaseHttpResponse(HttpStatusCode.OK));
            }
        }

        return Unauthorized();
    }
}
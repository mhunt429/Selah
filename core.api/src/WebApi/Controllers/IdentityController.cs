using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.ApplicationUser;
using Application.Identity;
using Domain.Models;
using Domain.ApiContracts;
using Domain.ApiContracts.Identity;
using Domain.Results;
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
        LoginResult result = await _mediatr.Send(request);

        if (!result.Success || result.AccessTokenResponse is null)
        {
            return Unauthorized();
        }

        if (request.RememberMe)
        {
            Response.Cookies.Append("x_session_id", result.AccessTokenResponse.SessionId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }

        Response.Cookies.Append("x_api_token", result.AccessTokenResponse.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
        });
        return Ok(result.AccessTokenResponse.ToBaseHttpResponse(HttpStatusCode.OK));
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

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var sessionIdHeader = Request.Cookies.FirstOrDefault(x => x.Key == "x_api_token");
        if (sessionIdHeader.Value == null)
        {
            return Forbid();
        }

        AppRequestContext? requestContext = Request.GetAppRequestContext();
        if (requestContext == null) return Forbid();

        return Ok();
    }
}
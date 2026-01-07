using System.Net;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Domain.ApiContracts;
using Domain.ApiContracts.Identity;
using Domain.Results;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Extensions;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController(IdentityService identityService, AppUserService userService)
    : ControllerBase
{
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

        ApplicationUser? result = await userService.GetUserById(userId);
        if (result == null)
        {
            return Unauthorized();
        }

        return Ok(result.ToBaseHttpResponse(HttpStatusCode.OK));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [EnableRateLimiting("PublicEndpointPolicy")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        LoginResult result = await identityService.Login(request);

        if (result is {Status: LoginStatus.Failed})
        {
            return Unauthorized();
        }

        if (request.RememberMe)
        {
            Response.Cookies.Append("x_session_id", result!.AccessTokenResponse!.SessionId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }

        Response.Cookies.Append("x_api_token", result!.AccessTokenResponse!.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
        });
        return Ok(result.AccessTokenResponse.ToBaseHttpResponse(HttpStatusCode.OK));
    }


    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [EnableRateLimiting("PublicEndpointPolicy")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        LoginResult result = await identityService.RefreshAccessToken(request);
        if (result is {Status: LoginStatus.Failed}) return Unauthorized();

        return Ok(result.AccessTokenResponse.ToBaseHttpResponse(HttpStatusCode.OK));
    }
}
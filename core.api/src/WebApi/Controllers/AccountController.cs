using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Domain.ApiContracts;
using Domain.ApiContracts.AccountRegistration;
using Domain.ApiContracts.Identity;
using Domain.Constants;
using Domain.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace WebApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [EnableRateLimiting(Constants.PublicEndpointPolicy)]
    [Route("api/[controller]")]
    public class AccountController(RegistrationService registrationService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterCommand([FromBody] AccountRegistrationRequest request)
        {
            ApiResponseResult<AccessTokenResponse> result = await registrationService.RegisterAccount(request);

            if (result.status == ResultStatus.Failed)
            {
                return BadRequest(new BaseHttpResponse<AccessTokenResponse>
                {
                    StatusCode = 400,
                    Data = null,
                    Errors = result.message?.Split(','),
                });
            }

            Response.Cookies.Append("x_api_token", result!.data!.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            return Ok(new BaseHttpResponse<AccessTokenResponse>
            {
                StatusCode = 200,
                Data = result.data,
            });
        }
    }
}
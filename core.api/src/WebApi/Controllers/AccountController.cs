using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Domain.ApiContracts;
using Domain.ApiContracts.AccountRegistration;
using Domain.ApiContracts.Identity;
using Domain.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly RegistrationService _registrationService;

        public AccountController(RegistrationService  registrationService)
        {
         _registrationService = registrationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountRegistrationRequest request)
        {
            ApiResponseResult<AccessTokenResponse> result = await _registrationService.RegisterAccount(request);

            if (result.status == ResultStatus.Failed)
            {
                return BadRequest(new BaseHttpResponse<AccessTokenResponse>
                {
                    StatusCode = 400,
                    Data = null,
                    Errors = result.message?.Split(','),
                });
            }

            Response.Cookies.Append("x_api_token", result.data.AccessToken, new CookieOptions
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
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Registration;
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
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterAccount.Command command)
        {
            ApiResponseResult<AccessTokenResponse> result = await _mediator.Send(command);

            if (result.status == ResultStatus.Failed)
            {
                return BadRequest(new BaseHttpResponse<AccessTokenResponse>
                {
                    StatusCode = 400,
                    Data = null,
                    Errors = result.message?.Split(','),
                });
            }

            Response.Cookies.Append("x_api_token", result.data.AccessToken.ToString(), new CookieOptions
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
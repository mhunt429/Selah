using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Application.Registration;
using Domain.ApiContracts.Identity;
using Domain.Models;
using WebApi.Controllers;

namespace WebApi.UnitTests.Controllers;

public class AccountControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();

    private AccountController _controller;


    public AccountControllerTests()
    {
        var appRequestContext = new AppRequestContext { UserId = 1 };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = "Bearer my_token";

        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
        };

        _controller = new AccountController(_mediatorMock.Object)
        { ControllerContext = controllerContext };
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnOkResult()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<RegisterAccount.Command>(), CancellationToken.None))
            .ReturnsAsync(
                new ApiResponseResult<AccessTokenResponse>(status: ResultStatus.Success, default, default));

        var command = new RegisterAccount.Command
        {
            FirstName = "Hingle",
            LastName = "McCringleberry",
            Email = "testing123@test.com",
            Password = "AStrongPassword!42",
            PasswordConfirmation = "AStrongPassword!42",
        };

        var result = await _controller.Register(command);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnBadRequestResult()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<RegisterAccount.Command>(), CancellationToken.None))
            .ReturnsAsync(
                new ApiResponseResult<AccessTokenResponse>(status: ResultStatus.Failed, default, default));

        var result = await _controller.Register(new RegisterAccount.Command());
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
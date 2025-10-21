using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Application.Identity;
using Application.ApplicationUser;
using Domain.ApiContracts;
using Domain.ApiContracts.Identity;
using Domain.Models;
using WebApi.Controllers;

namespace WebApi.UnitTests;

public class IdentityControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();

    private IdentityController _controller;

    public IdentityControllerTests()
    {
        var appRequestContext = new AppRequestContext { UserId = 1 };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = "Bearer my_token";

        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
        };

        _controller = new IdentityController(_mediatorMock.Object)
        { ControllerContext = controllerContext };
    }

    [Fact]
    public async Task GetUserAsync_ShouldReturnUser()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetUserById.Query>(), CancellationToken.None))
            .ReturnsAsync(new ApplicationUser
            {
                Id = 1,
                AccountId = 2,
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User"
            });
        var result = await _controller.GetCurrentUser();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetUserAsync_ShouldUnAuthorized_WhenUserIsNotFound()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetUserById.Query>(), default))
            .ReturnsAsync((ApplicationUser)null);

        var result = await _controller.GetCurrentUser();
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Login_ShouldReturnAccessToken()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<UserLogin.Command>(), default))
            .ReturnsAsync(new UserLogin.Response {AccessToken = new AccessTokenResponse{AccessToken = "ABC123"}});

        var result = await _controller.Login(new UserLogin.Command());
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Login_ShouldReturnUnAuthorized_OnInvalidLoginRequest()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<UserLogin.Command>(), default))
            .ReturnsAsync((UserLogin.Response)null);
        var result = await _controller.Login(new UserLogin.Command());
        Assert.IsType<UnauthorizedResult>(result);
    }
}
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Application.AccountConnector;
using Domain.Models;
using Domain.Models.Plaid;
using WebApi.Controllers;

namespace WebApi.UnitTests.Controllers;

public class ConnectorControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();

    private ConnectorController _controller;

    public ConnectorControllerTests()
    {
        var appRequestContext = new AppRequestContext { UserId = 1 };

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = "Bearer my_token";

        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
        };

        _controller = new ConnectorController(_mediatorMock.Object)
            { ControllerContext = controllerContext };
    }

    [Fact]
    public async Task LinkAccount_ShouldReturnSuccess_WhenLinkAccountIsSuccessful()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<CreateLinkToken.Command>(), CancellationToken.None))
            .ReturnsAsync(new PlaidLinkToken{LinkToken = "Token123"});

        var result = await _controller.GetLinkToken();
        Assert.IsType<OkObjectResult>(result);
    }
  
}
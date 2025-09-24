using System.Net;
using Application.Banking;
using Domain.ApiContracts;
using Domain.ApiContracts.Banking;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;

namespace WebApi.UnitTests.Controllers;

public class BankingControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();

    private readonly BankingController _bankingController;

    public BankingControllerTests()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = "Bearer my_token";
        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
        };

        _bankingController = new BankingController(_mediatorMock.Object)
            { ControllerContext = controllerContext };
    }

    [Fact]
    public async Task Get_Account_Returns_200_WhenValid_RequestContextIsSet()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetAccountsByUserId.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FinancialAccountDto>());

        var result = await _bankingController.GetAccounts();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<BaseHttpResponse<IEnumerable<FinancialAccountDto>>>(okResult.Value);
    }
}
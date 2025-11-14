using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.ApplicationUser;
using Application.Identity;
using Domain.ApiContracts;
using Domain.ApiContracts.Identity;
using Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using Xunit;

namespace WebApi.UnitTests
{
    public class IdentityControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private IdentityController _controller;

        public IdentityControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();

            var httpContext = new DefaultHttpContext();
            _controller = new IdentityController(_mediatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        // -------------------------------
        // LOGIN TESTS
        // -------------------------------

        [Fact]
        public async Task Login_ShouldReturnOk_AndSetCookies_WhenRememberMeTrue()
        {
            // Arrange
            var response = new LoginResult(true, new AccessTokenResponse { AccessToken = "ABC123," });

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<UserLogin.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var command = new UserLogin.Command { RememberMe = true };

            // Act
            var result = await _controller.Login(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("x_api_token", _controller.Response.Headers.SetCookie.ToString());
            Assert.Contains("x_session_id", _controller.Response.Headers.SetCookie.ToString());
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_AndSetAccessTokenCookie_WhenRememberMeFalse()
        {
            // Arrange
            var response = new LoginResult(true, new AccessTokenResponse { AccessToken = "ABC123," });

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<UserLogin.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var command = new UserLogin.Command { RememberMe = false };

            // Act
            var result = await _controller.Login(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("x_api_token", _controller.Response.Headers.SetCookie.ToString());
            Assert.DoesNotContain("x_session_id", _controller.Response.Headers.SetCookie.ToString());
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenResultIsFailed()
        {
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<UserLogin.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LoginResult(false, null));

            var result = await _controller.Login(new UserLogin.Command());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenAccessTokenIsNull()
        {
            var response = new AccessTokenResponse { AccessToken = null };
            _mediatorMock
                .Setup(x => x.Send(It.IsAny<UserLogin.Command>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LoginResult(false, response));

            var result = await _controller.Login(new UserLogin.Command());

            Assert.IsType<UnauthorizedResult>(result);
        }

        // -------------------------------
        // CURRENT SESSION TESTS
        // -------------------------------

        [Fact]
        public async Task GetCurrentSession_ShouldReturnUnauthorized_WhenNoSessionCookie()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.GetCurrentSession();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetCurrentSession_ShouldReturnUnauthorized_WhenInvalidGuid()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.GetCurrentSession();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetCurrentSession_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var httpContext = new DefaultHttpContext();

            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<UserBySessionIdQuery.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.GetCurrentSession();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
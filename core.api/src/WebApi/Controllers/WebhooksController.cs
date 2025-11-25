using System.Threading.Channels;
using Domain.MessageContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("PublicEndpointPolicy")]
public class WebhooksController(ChannelWriter<PlaidWebhookEvent> publisher) : ControllerBase
{
    [HttpPost("plaid")]
    public async Task<IActionResult> ProcessPlaidWebhook()
    {
        await publisher.WriteAsync(new PlaidWebhookEvent
        {
            EventId = Guid.NewGuid(),
            ItemId = "ABC-123"
        });

        return NoContent();
    }
}
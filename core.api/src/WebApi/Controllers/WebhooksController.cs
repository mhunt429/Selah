using System.Threading.Channels;
using Domain.MessageContracts;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private ChannelWriter<PlaidWebhookEvent> _publisher;

    public WebhooksController(ChannelWriter<PlaidWebhookEvent> publisher)
    {
        _publisher = publisher;
    }

    [HttpPost("plaid")]
    public async Task<IActionResult> ProcessPlaidWebhook()
    {
        await _publisher.WriteAsync(new PlaidWebhookEvent
        {
            EventId = Guid.NewGuid(),
            ItemId = "ABC-123"
        });

        return NoContent();
    }
}
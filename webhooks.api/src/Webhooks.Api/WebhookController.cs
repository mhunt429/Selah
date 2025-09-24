using AWS.Messaging;
using Microsoft.AspNetCore.Mvc;
using Webhooks.Core.Enums;
using Webhooks.Core.MessageContracts;

namespace Webhooks.Api;

[ApiController]
[Route("[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IMessagePublisher _messagePublisher;
    public WebhookController(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    [HttpPost("plaid")]
    public async Task<IActionResult> PublishPlaidWebhook()
    {
        await _messagePublisher.PublishAsync(new PlaidWebhookEvent
        {
            DateSent = DateTimeOffset.UtcNow,
            ItemId = "Test-Id",
            EventId = Guid.NewGuid(),
            PlaidWebhookType = PlaidWebhookType.SYNC_UPDATES_AVAILABLE
        });
        return NoContent();
    }
}
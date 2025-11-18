using Akka.Actor;
using Akka.DependencyInjection;
using Domain.MessageContracts;
using Infrastructure.Actors;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly IActorRef _plaidWebhookActor;

    public WebhooksController(IActorRef plaidWebhookActor)
    {
        _plaidWebhookActor = plaidWebhookActor;
    }

    /// <summary>
    /// Plaid has a 10-second timeout for webhooks so they prefer a
    /// quick response so we can return a 204 to them and then process the webhook
    /// through our asynchronous actor system.
    ///
    /// TODO implement this more and then remove the SQS/SNS logic
    /// </summary>
    /// <returns></returns>
    [HttpPost("plaid")]
    public async Task<IActionResult> ProcessPlaidWebhook()
    {
        _plaidWebhookActor.Tell(new PlaidWebhookEvent
        {
            EventId = Guid.NewGuid(),
            ItemId = "ABC-123" // Just for testing
        });
        return NoContent();
    }
}
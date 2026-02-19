using System.Threading.Channels;
using Domain.ApiContracts.Connector;
using Domain.Constants;
using Domain.Events;
using Domain.MessageContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Filters;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting(Constants.PublicEndpointPolicy)]
public class WebhooksController(ChannelWriter<PlaidWebhookEvent> publisher) : ControllerBase
{
    [HttpPost("plaid")]
   // [TypeFilter(typeof(PlaidWebhookVerificationActionFilter))]
    public async Task<IActionResult> ProcessPlaidWebhookCommand([FromBody] PlaidWebhookRequest request)
    {
        PlaidWebhookType webhookType;
        if (Enum.TryParse(request.WebhookCode, true, out webhookType))
        {
            await publisher.WriteAsync(new PlaidWebhookEvent
            {
                EventId = Guid.NewGuid(),
                ItemId = request.ItemId,
                PlaidWebhookType = webhookType ,
            });
            return NoContent();
        }
        return BadRequest("Invalid Webhook Code");
    }
}
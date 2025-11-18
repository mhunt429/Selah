using AWS.Messaging;
using Domain.MessageContracts;
using Microsoft.Extensions.Logging;

namespace Application.MessageHandlers;

public class PlaidWebhookEventMessageHandler : IMessageHandler<PlaidWebhookEvent>
{
    private readonly ILogger<PlaidWebhookEventMessageHandler> _logger;

    public PlaidWebhookEventMessageHandler(ILogger<PlaidWebhookEventMessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<PlaidWebhookEvent> messageEnvelope,
        CancellationToken token = default)
    {
        var plaidWebhookEvent = messageEnvelope.Message;
        try
        {
            _logger.LogInformation($"Processing Plaid Webhook Event: {plaidWebhookEvent.EventId}");
            return MessageProcessStatus.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Processing Plaid Webhook Event {plaidWebhookEvent.EventId}");
            return MessageProcessStatus.Failed();
        }
    }
}
using System.Threading.Channels;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public sealed class WebhookService(ChannelWriter<ConnectorDataSyncEvent> publisher, ILogger<WebhookService> logger)
{
    private ChannelWriter<ConnectorDataSyncEvent> _publisher = publisher;
    private ILogger<WebhookService> _logger = logger;

    public async Task ProcessPlaidWebhook()
    {
        
    }
}
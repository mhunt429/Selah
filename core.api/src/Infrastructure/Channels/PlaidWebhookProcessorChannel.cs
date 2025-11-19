using System.Threading.Channels;
using Domain.MessageContracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Channels;

public class PlaidWebhookProcessorChannel : BackgroundService
{
    private readonly ChannelReader<PlaidWebhookEvent> _channelReader;
    private readonly ILogger<PlaidWebhookProcessorChannel> _logger;

    public PlaidWebhookProcessorChannel(ChannelReader<PlaidWebhookEvent> channelReader,
        ILogger<PlaidWebhookProcessorChannel> logger)
    {
        _channelReader = channelReader;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _channelReader.WaitToReadAsync(stoppingToken))
        {
            while (_channelReader.TryRead(out var item))
            {
                _logger.LogInformation("Message is `{Message}`", item.EventId);
            }
        }
    }
}
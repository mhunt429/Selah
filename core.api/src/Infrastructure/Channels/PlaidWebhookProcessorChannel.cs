using System.Threading.Channels;
using Domain.MessageContracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Channels;

public class PlaidWebhookProcessorChannel(
    ChannelReader<PlaidWebhookEvent> channelReader,
    ILogger<PlaidWebhookProcessorChannel> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await channelReader.WaitToReadAsync(stoppingToken))
        {
            while (channelReader.TryRead(out var item))
            {
                logger.LogInformation("Message is `{Message}`", item.EventId);
            }
        }
       
    }
}
using System.Threading.Channels;
using Domain.MessageContracts;
using Infrastructure.Channels;

namespace WebApi.Extensions;

public static class ChannelExtensions
{
    public static IServiceCollection AddChannelServices(this IServiceCollection services)
    {
        services.AddSingleton(Channel.CreateBounded<PlaidWebhookEvent>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            AllowSynchronousContinuations = false,
        }));

        services.AddSingleton(c => c.GetRequiredService<Channel<PlaidWebhookEvent>>().Reader);
        services.AddSingleton(c => c.GetRequiredService<Channel<PlaidWebhookEvent>>().Writer);
        services.AddHostedService<PlaidWebhookProcessorChannel>();
        
        return services;
    }
}
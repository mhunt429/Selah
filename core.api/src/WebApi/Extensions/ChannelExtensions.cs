using System.Threading.Channels;
using Domain.Events;
using Domain.MessageContracts;
using Infrastructure.Channels;

namespace WebApi.Extensions;

public static class ChannelExtensions
{
    public static IServiceCollection AddChannelServices(this IServiceCollection services)
    {
        services.AddSingleton(Channel.CreateBounded<PlaidWebhookEvent>(new BoundedChannelOptions(50)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false,
        }));

        services.AddSingleton(Channel.CreateBounded<ConnectorDataSyncEvent>(new BoundedChannelOptions(50)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false,
        }));

        services.AddSingleton(c => c.GetRequiredService<Channel<PlaidWebhookEvent>>().Reader);
        services.AddSingleton(c => c.GetRequiredService<Channel<PlaidWebhookEvent>>().Writer);

        services.AddSingleton(c => c.GetRequiredService<Channel<ConnectorDataSyncEvent>>().Reader);
        services.AddSingleton(c => c.GetRequiredService<Channel<ConnectorDataSyncEvent>>().Writer);

        services.AddHostedService<PlaidWebhookProcessorChannel>();
        services.AddHostedService<AccountBalanceImportChannel>();

        return services;
    }
}
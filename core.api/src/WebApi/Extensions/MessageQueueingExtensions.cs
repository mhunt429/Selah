using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Application.MessageHandlers;
using Domain.MessageContracts;

namespace WebApi.Extensions;

public static class MessageQueueingExtensions
{
    public static IServiceCollection AddMessageQueuing(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(new AWSOptions
        {
            Profile = "default",
            Region = RegionEndpoint.USEast1
        });

        string plaidWebHookQueue = configuration.GetValue<string>("AwsConfig:PlaidWebhookMessageQueue");

        services.AddAWSMessageBus(bus =>
        {
            bus.AddSQSPoller(plaidWebHookQueue);
            bus.AddMessageHandler<PlaidWebhookEventMessageHandler, PlaidWebhookEvent>();
        });
        return services;
    }
}
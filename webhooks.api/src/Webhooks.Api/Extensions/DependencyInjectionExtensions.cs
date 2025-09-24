using Webhooks.Core.Config;
using Webhooks.Core.MessageContracts;

namespace Webhooks.Api.Extensions;

public  static class DependencyInjectionExtensions
{
    public static IServiceCollection RegisterMessageQueuing(this IServiceCollection services, IConfiguration configuration)
    {
        AwsMessageSettings awsMessageSettings = configuration.GetSection("AwsMessageSettings").Get<AwsMessageSettings>();
        if (awsMessageSettings == null)
        {
            throw new ArgumentNullException(nameof(awsMessageSettings));
        }
        services.AddAWSMessageBus(bus =>
        {
            bus.AddSNSPublisher<PlaidWebhookEvent>(awsMessageSettings.PlaidWebhookTopic);
        });
        return services;
    }
}
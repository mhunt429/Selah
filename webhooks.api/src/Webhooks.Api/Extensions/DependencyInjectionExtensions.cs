using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using Webhooks.Core.Config;
using Webhooks.Core.MessageContracts;

namespace Webhooks.Api.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection RegisterMessageQueuing(this IServiceCollection services,
        IConfiguration configuration)
    {
        AwsMessageSettings awsMessageSettings =
            configuration.GetSection("AwsMessageSettings").Get<AwsMessageSettings>();
        if (awsMessageSettings == null)
        {
            throw new ArgumentNullException(nameof(awsMessageSettings));
        }

        services.AddDefaultAWSOptions(new AWSOptions
        {
            Profile = "default",
            Region = RegionEndpoint.USEast1
        });

        services.AddAWSMessageBus(bus =>
        {
            bus.AddSNSPublisher<PlaidWebhookEvent>(awsMessageSettings.PlaidWebhookTopic);
        });
        
       services.AddAWSService<IAmazonSQS>();
        return services;
    }
}
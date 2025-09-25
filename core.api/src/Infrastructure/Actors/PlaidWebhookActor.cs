using Akka.Actor;
using Domain.MessageContracts;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Actors;

public class PlaidWebhookActor : ReceiveActor
{
    private readonly ILogger<PlaidWebhookActor> _logger;

    public PlaidWebhookActor(ILogger<PlaidWebhookActor> logger)
    {
        _logger = logger;

        ReceiveAsync<PlaidWebhookEvent>(async message =>
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            _logger.LogInformation("Processing PlaidWebhookEvent: {Message}", message);
            //Sender.Tell("Received PlaidWebhookEvent!!");
        });
    }
}
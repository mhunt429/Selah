using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Actors;

public class SqsMessageActor : ReceiveActor
{
    private readonly ILogger<SqsMessageActor> _logger;

    public SqsMessageActor(ILogger<SqsMessageActor> logger)
    {
        _logger = logger;

        //TODO Make this user concrete class/record types. This is just for testing the initial AWS SQS functionality
        ReceiveAsync<string>(message =>
        {
            _logger.LogInformation("Processing SQS message: {Message}", message);

            Sender.Tell(new Ack());

            return Task.CompletedTask;
        });
    }

    public sealed class Ack
    {
    }
}
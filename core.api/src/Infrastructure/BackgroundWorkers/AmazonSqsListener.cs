using Microsoft.Extensions.Hosting;
using Akka.Actor;
using Akka.DependencyInjection;
using Amazon.SQS;
using Amazon.SQS.Model;
using Infrastructure.Actors;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Infrastructure.BackgroundWorkers;

public class AmazonSqsListener: BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly string _queueUrl;
    private readonly ILogger<AmazonSqsListener> _logger;
    private readonly IActorRef _messageActor;
    
    public AmazonSqsListener(
        IAmazonSQS sqs,
        ILogger<AmazonSqsListener> logger,
        ActorSystem actorSystem,
        IServiceProvider sp)
    {
        _sqs = sqs;
        _logger = logger;
        _queueUrl = ""; //TODO set this up 
        
        var resolver = DependencyResolver.For(actorSystem);
        _messageActor = actorSystem.ActorOf(resolver.Props<SqsMessageActor>(), "sqsMessageActor");
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting SQS listener on {QueueUrl}", _queueUrl);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var receiveRequest = new ReceiveMessageRequest{
                    QueueUrl= _queueUrl,
                    MaxNumberOfMessages=1,
                    WaitTimeSeconds=TimeSpan.FromSeconds(10).Seconds,
                };

                var response = await _sqs.ReceiveMessageAsync(receiveRequest, stoppingToken);

                foreach (var msg in response.Messages)
                {
                    _messageActor.Tell(msg.Body);

                    await _sqs.DeleteMessageAsync(_queueUrl, msg.ReceiptHandle, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error polling SQS");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
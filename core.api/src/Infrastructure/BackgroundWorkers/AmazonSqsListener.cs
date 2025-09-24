using Microsoft.Extensions.Hosting;
using Akka.Actor;
using Akka.DependencyInjection;
using Amazon.SQS;
using Amazon.SQS.Model;
using Infrastructure.Actors;
using Microsoft.Extensions.Logging;
namespace Infrastructure.BackgroundWorkers;

public class AmazonSqsListener: BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly string _queueUrl;
    private readonly ILogger<AmazonSqsListener> _logger;
    private readonly IActorRef _messageActor;
    private readonly int _initialDelaySeconds = 5;
    private readonly int _maxDelaySeconds = 60;
    public AmazonSqsListener(
        IAmazonSQS sqs,
        ILogger<AmazonSqsListener> logger,
        ActorSystem actorSystem
     )
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
        int backoffDelay = _initialDelaySeconds;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var receiveRequest = new ReceiveMessageRequest{
                    QueueUrl= _queueUrl,
                    MaxNumberOfMessages=10,
                    WaitTimeSeconds=TimeSpan.FromSeconds(20).Seconds,
                };

                var response = await _sqs.ReceiveMessageAsync(receiveRequest, stoppingToken);
                
                if (response.Messages.Count == 0)
                {
                    _logger.LogDebug("No messages in queue, backing off for {Delay}s", backoffDelay);
                    await Task.Delay(TimeSpan.FromSeconds(backoffDelay), stoppingToken);

                    backoffDelay = Math.Min(backoffDelay * 2, _maxDelaySeconds); 
                    continue;
                }
                
                backoffDelay = _initialDelaySeconds;

                foreach (var msg in response.Messages)
                {
                    SqsMessageActor.Ack akkaResponse = await _messageActor.Ask<SqsMessageActor.Ack>(msg.Body);

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
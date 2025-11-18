using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Streams;
using Infrastructure.Actors;

namespace WebApi.Extensions;

public static class AkkaExtensions
{
    /// <summary>
    /// Extensions methods for Akka dependency injection
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterActorSystem(this IServiceCollection services)
    {
        var bootstrap = BootstrapSetup.Create();
        var di = DependencyResolverSetup.Create(services.BuildServiceProvider());
        var actorSystemSetup = bootstrap.And(di);

        var actorSystem = ActorSystem.Create("SelahActorSystem", actorSystemSetup);
        services.AddSingleton(actorSystem);

        services.AddSingleton<IMaterializer>(_ => actorSystem.Materializer());

        var resolver = DependencyResolver.For(actorSystem);
        var plaidActorRef = actorSystem.ActorOf(resolver.Props<PlaidWebhookActor>(), "plaidWebhookActor");
        services.AddSingleton(plaidActorRef);

        var accountBalanceSyncActorRef = actorSystem.ActorOf(resolver.Props<AccountBalanceSyncActor>());
        services.AddSingleton(accountBalanceSyncActorRef);

        return services;
    }
}
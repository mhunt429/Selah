using Akka.Actor;
using Domain.Actors;

namespace Infrastructure.Actors;

public class AccountBalanceSyncActor : ReceiveActor
{
    public AccountBalanceSyncActor()
    {
        ReceiveAsync<ImportAccountBalanceActorCommand>(command =>
        {

            return Task.CompletedTask;
        });
    }

    private async Task<string> ImportBalances(IEnumerable<string> accountIds)
    {
        await Task.Delay(100); // simulate I/O
        return $"Imported {accountIds.Count()} balances";
    }
}
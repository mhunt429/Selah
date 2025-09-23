using Akka.Actor;
using Domain.Actors;

namespace Infrastructure.Actors;

public class AccountBalanceImportActor : ReceiveActor
{
    public AccountBalanceImportActor()
    {
        Receive<ImportAccountBalanceActorCommand>(async command =>
        {
            if (command.InstitutionId is null) throw new ArgumentNullException(nameof(command.InstitutionId));

            await ImportBalances(command.InstitutionId)
                .PipeTo(Sender);
            
            return 1;
        });
    }

    private async Task<string> ImportBalances(IEnumerable<string> accountIds)
    {
        await Task.Delay(100); // simulate I/O
        return $"Imported {accountIds.Count()} balances";
    }
}
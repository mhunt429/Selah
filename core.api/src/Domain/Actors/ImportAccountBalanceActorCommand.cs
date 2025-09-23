namespace Domain.Actors;

public record ImportAccountBalanceActorCommand(
    int UserId,
    //Optional since this can either be called for all account by a user or for a specific institution
    int? InstitutionId
);
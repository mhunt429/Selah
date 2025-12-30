namespace Domain.ApiContracts.Analytics;

public record DashboardSummary(
    IncomeSummary Income,
    SpendingSummary Spending,
    SavingsSummary Savings
);

public record IncomeSummary(
    decimal Income,
    double Delta,
    DashboardFilterRange FilterRange
);

public record SpendingSummary(
    decimal Income,
    double Delta,
    DashboardFilterRange FilterRange
);

public record SavingsSummary(
    decimal Income,
    decimal Delta,
    DashboardFilterRange FilterRange
);

public enum DashboardFilterRange
{
    Weekly,
    Monthly,
    Yearly,
    CustomRange
}
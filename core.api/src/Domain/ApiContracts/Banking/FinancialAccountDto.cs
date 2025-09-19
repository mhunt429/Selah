namespace Domain.ApiContracts.Banking;

public class FinancialAccountDto
{
    public int Id { get; set; }

    public decimal CurrentBalance { get; set; }

    public string AccountMask { get; set; } = "";

    public required string DisplayName { get; set; }

    public string OfficialName { get; set; } = "";

    public required string Subtype { get; set; }

    public DateTimeOffset LastApiSyncTime { get; set; }
}
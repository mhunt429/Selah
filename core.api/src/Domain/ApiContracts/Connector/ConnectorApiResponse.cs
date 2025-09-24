namespace Domain.ApiContracts.Connector;

public class ConnectorApiResponse
{
    public int InstitutionId { get; set; }
    
    public required string InstitutionName { get; set; } 
    
    public DateTimeOffset DateConnected { get; set; }
}
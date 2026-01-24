namespace Domain.Configuration;

public class PlaidConfig
{
    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }

    public required string BaseUrl { get; set; }
    
    public required int MaxDaysRequested { get; set; }
    
    public string? WebhookUrl { get; set; }
}
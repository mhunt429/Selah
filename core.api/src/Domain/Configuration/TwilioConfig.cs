namespace Domain.Configuration;

public class TwilioConfig
{
    public required string ApiToken { get; set; }
    
    public  required string AccountSid { get; set; }  
    
    public required string AppNumber { get; set; }
}
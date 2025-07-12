namespace Domain.Models;

public class AppRequestContext
{
    public Guid UserId { get; set; } 
    public string TraceId { get; set; } = string.Empty;  
    public string IpAddress { get; set; }= string.Empty;  
}
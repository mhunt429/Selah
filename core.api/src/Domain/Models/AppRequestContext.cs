namespace Domain.Models;

public class AppRequestContext
{
    public int UserId { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}
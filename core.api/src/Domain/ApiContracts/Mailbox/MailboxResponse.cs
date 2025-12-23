namespace Domain.ApiContracts.Mailbox;

public class MailboxResponse
{
    public int Id { get; set; }

    public bool Unread { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    
    public required string MessageKey { get; set; }
    
    public required string MessageBody { get; set; }
    
}

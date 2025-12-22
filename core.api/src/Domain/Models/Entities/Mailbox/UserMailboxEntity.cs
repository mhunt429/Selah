using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities.Mailbox;

[Table("user_mailbox")]
public class UserMailboxEntity: BaseAuditFields
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("message_key")]
    public required string MessageKey { get; set; }
    
    [Column("user_id")]
    public int UserId { get; set; }
    
    [Column("has_seen")]
    public bool HasSeen { get; set; }
    
    [Column("message_body")]
    public required string MessageBody { get; set; }
    
    [Column("action_type")]
    public required string ActionType { get; set; }
}
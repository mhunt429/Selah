using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities;

public class BaseAuditFields
{
    [Column("app_last_changed_by")] public required int AppLastChangedBy { get; set; }
    
    [Column("original_insert")] public DateTimeOffset OriginalInsert { get; set; }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Entities;

public class BaseAuditFields
{
    [Column("original_insert")] public required DateTimeOffset OriginalInsert { get; set; }

    [Column("last_update")] public DateTimeOffset LastUpdate { get; private set; } = DateTimeOffset.UtcNow;

    [Column("app_last_changed_by")] public int AppLastChangedBy { get; set; }
}
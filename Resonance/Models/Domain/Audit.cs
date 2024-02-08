using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Audit
{
    public long Id { get; set; }

    public string TableName { get; set; } = null!;

    public long RecordId { get; set; }

    public string ColumnName { get; set; } = null!;

    public string OldValue { get; set; } = null!;

    public string NewValue { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;
}

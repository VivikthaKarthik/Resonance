using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Audit
{
    public int Id { get; set; }

    public string TableName { get; set; } = null!;

    public int ParentId { get; set; }

    public string ColumnName { get; set; } = null!;

    public string OldValue { get; set; } = null!;

    public string NewValue { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; } = null!;
}

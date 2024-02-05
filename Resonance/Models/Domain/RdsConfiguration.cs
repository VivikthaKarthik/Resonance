using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class RdsConfiguration
{
    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string? Description { get; set; }

    public bool CustomerModifiable { get; set; }

    public bool CustomerVisible { get; set; }

    public long RecordVersion { get; set; }

    public int? MaxValue { get; set; }

    public int? MinValue { get; set; }
}

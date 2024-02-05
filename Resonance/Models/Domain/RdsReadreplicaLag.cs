using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class RdsReadreplicaLag
{
    public string AgName { get; set; } = null!;

    public long? LagSeconds { get; set; }

    public DateTime UpdatedAt { get; set; }
}

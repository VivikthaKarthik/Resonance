using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class DropdbActivity
{
    public Guid FamilyGuid { get; set; }

    public string DatabaseName { get; set; } = null!;

    public DateTime DroppedTime { get; set; }

    public string Lifecycle { get; set; } = null!;
}

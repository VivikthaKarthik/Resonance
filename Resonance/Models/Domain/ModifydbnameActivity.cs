using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class ModifydbnameActivity
{
    public Guid FamilyGuid { get; set; }

    public string OldDbName { get; set; } = null!;

    public string NewDbName { get; set; } = null!;

    public DateTime DbRenamedAt { get; set; }

    public string Lifecycle { get; set; } = null!;
}

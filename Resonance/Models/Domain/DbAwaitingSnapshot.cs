using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class DbAwaitingSnapshot
{
    public Guid FamilyGuid { get; set; }

    public string RootCause { get; set; } = null!;
}

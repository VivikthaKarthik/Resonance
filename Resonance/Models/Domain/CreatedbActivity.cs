using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class CreatedbActivity
{
    public Guid FamilyGuid { get; set; }

    public string DatabaseName { get; set; } = null!;

    public DateTime ProperlyCreatedTime { get; set; }
}

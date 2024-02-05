using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class DbMapping
{
    public Guid FamilyGuid { get; set; }

    public string DatabaseName { get; set; } = null!;

    public DateTime CreatedTime { get; set; }
}

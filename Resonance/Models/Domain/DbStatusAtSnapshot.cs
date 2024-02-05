using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class DbStatusAtSnapshot
{
    public Guid RdsDbUniqueId { get; set; }

    public string DbStatus { get; set; } = null!;
}

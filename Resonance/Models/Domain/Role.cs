using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Role
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ResoUser> ResoUsers { get; } = new List<ResoUser>();
}

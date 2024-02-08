﻿using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Role
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual ICollection<User> Users { get; } = new List<User>();
}

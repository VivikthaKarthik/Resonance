﻿using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class VwClass
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Course { get; set; } = null!;
}

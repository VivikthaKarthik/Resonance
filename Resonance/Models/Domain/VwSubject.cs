using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class VwSubject
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Class { get; set; } = null!;

    public string Course { get; set; } = null!;

    public string? ColorCode { get; set; }
}

using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class VwSubject
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long ClassId { get; set; }

    public string Class { get; set; } = null!;

    public long CourseId { get; set; }

    public string Course { get; set; } = null!;

    public string? ColorCode { get; set; }

    public string? Thumbnail { get; set; }
}

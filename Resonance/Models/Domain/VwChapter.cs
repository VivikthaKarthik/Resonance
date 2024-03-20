using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class VwChapter
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Class { get; set; } = null!;

    public string Course { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsRecommended { get; set; }
}

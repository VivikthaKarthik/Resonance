using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class VwTopic
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Chapter { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Class { get; set; } = null!;

    public string Course { get; set; } = null!;

    public string? Description { get; set; }

    public string Thumbnail { get; set; } = null!;
}

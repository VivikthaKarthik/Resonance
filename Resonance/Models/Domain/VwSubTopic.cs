using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class VwSubTopic
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public string SourceUrl { get; set; } = null!;

    public bool HomeDisplay { get; set; }

    public long? TopicId { get; set; }

    public string? Topic { get; set; }

    public long ChapterId { get; set; }

    public string Chapter { get; set; } = null!;

    public long SubjectId { get; set; }

    public string Subject { get; set; } = null!;

    public long ClassId { get; set; }

    public string Class { get; set; } = null!;

    public long CourseId { get; set; }

    public string Course { get; set; } = null!;
}

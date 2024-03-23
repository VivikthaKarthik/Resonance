﻿using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class VwChapter
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long SubjectId { get; set; }

    public string Subject { get; set; } = null!;

    public long ClassId { get; set; }

    public string Class { get; set; } = null!;

    public long CourseId { get; set; }

    public string Course { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsRecommended { get; set; }
}

﻿using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class SubjectCourse
{
    public long Id { get; set; }

    public long CourseId { get; set; }

    public long SubjectId { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}

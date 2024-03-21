﻿using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Course
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual ICollection<AssessmentConfiguration> AssessmentConfigurations { get; } = new List<AssessmentConfiguration>();

    public virtual ICollection<Class> Classes { get; } = new List<Class>();

    public virtual ICollection<ScheduledExam> ScheduledExams { get; } = new List<ScheduledExam>();
}

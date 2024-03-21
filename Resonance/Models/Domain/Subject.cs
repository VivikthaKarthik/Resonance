using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Subject
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long ClassId { get; set; }

    public string? ColorCode { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<ScheduledExam> ScheduledExams { get; } = new List<ScheduledExam>();
}

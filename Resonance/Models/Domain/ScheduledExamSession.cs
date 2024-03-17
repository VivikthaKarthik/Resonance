using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class ScheduledExamSession
{
    public long Id { get; set; }

    public long ScheduledExamId { get; set; }

    public long StudentId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool? Result { get; set; }

    public virtual ScheduledExam ScheduledExam { get; set; } = null!;

    public virtual ICollection<ScheduledExamResult> ScheduledExamResults { get; } = new List<ScheduledExamResult>();

    public virtual Student Student { get; set; } = null!;
}

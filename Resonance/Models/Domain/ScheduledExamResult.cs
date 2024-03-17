using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class ScheduledExamResult
{
    public long Id { get; set; }

    public long ScheduledExamSessionId { get; set; }

    public long QuestionId { get; set; }

    public string? SelectedAnswer { get; set; }

    public bool? Result { get; set; }

    public virtual ScheduledExamQuestion Question { get; set; } = null!;

    public virtual ScheduledExamSession ScheduledExamSession { get; set; } = null!;
}

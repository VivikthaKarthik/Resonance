using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class ScheduledExamResult
{
    public long Id { get; set; }

    public long ScheduledExamId { get; set; }

    public long StudentId { get; set; }

    public long QuestionId { get; set; }

    public string? SelectedAnswer { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool? Result { get; set; }

    public long? DifficultyLevelId { get; set; }

    public virtual ScheduledExamQuestion Question { get; set; } = null!;

    public virtual ScheduledExam ScheduledExam { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}

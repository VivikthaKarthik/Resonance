using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class ScheduledExamQuestion
{
    public long Id { get; set; }

    public long? ScheduledExamId { get; set; }

    public string Question { get; set; } = null!;

    public string FirstAnswer { get; set; } = null!;

    public string SecondAnswer { get; set; } = null!;

    public string ThirdAnswer { get; set; } = null!;

    public string FourthAnswer { get; set; } = null!;

    public string CorrectAnswer { get; set; } = null!;

    public string? Explanation { get; set; }

    public long? DifficultyLevelId { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual ScheduledExam? ScheduledExam { get; set; }

    public virtual ICollection<ScheduledExamResult> ScheduledExamResults { get; } = new List<ScheduledExamResult>();
}

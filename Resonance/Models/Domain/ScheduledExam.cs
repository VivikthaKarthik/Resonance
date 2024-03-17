using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class ScheduledExam
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int MarksPerQuestion { get; set; }

    public int NegativeMarksPerQuestion { get; set; }

    public int MaxAllowedTime { get; set; }

    public long CourseId { get; set; }

    public long SubjectId { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<ScheduledExamQuestion> ScheduledExamQuestions { get; } = new List<ScheduledExamQuestion>();

    public virtual ICollection<ScheduledExamResult> ScheduledExamResults { get; } = new List<ScheduledExamResult>();

    public virtual Subject Subject { get; set; } = null!;
}

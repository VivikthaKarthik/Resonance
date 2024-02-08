using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Exam
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int NumberOfQuestions { get; set; }

    public int TotalMarks { get; set; }

    public int PassMarks { get; set; }

    public DateTime ScheduledOn { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual ICollection<ExamResult> ExamResults { get; } = new List<ExamResult>();
}

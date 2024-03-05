using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class AssessmentConfiguration
{
    public long Id { get; set; }

    public long CourseId { get; set; }

    public int MaximumQuestions { get; set; }

    public int MarksPerQuestion { get; set; }

    public bool HasNegativeMarking { get; set; }

    public int? NegativeMarksPerQuestion { get; set; }

    public int PassMarkks { get; set; }

    public int? TimeDuration { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual Course Course { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class AssessmentSession
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public long AssessmentLevelId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool? Result { get; set; }

    public virtual AssessmentLevel AssessmentLevel { get; set; } = null!;

    public virtual ICollection<AssessmentSessionQuestion> AssessmentSessionQuestions { get; } = new List<AssessmentSessionQuestion>();

    public virtual Student Student { get; set; } = null!;
}

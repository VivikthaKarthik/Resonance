using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class AssessmentSessionQuestion
{
    public long Id { get; set; }

    public long AssessmentSessionId { get; set; }

    public long QuestionId { get; set; }

    public bool? Result { get; set; }

    public virtual QuestionBank Id1 { get; set; } = null!;

    public virtual AssessmentSession IdNavigation { get; set; } = null!;
}

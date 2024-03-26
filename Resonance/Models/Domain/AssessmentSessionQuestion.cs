using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class AssessmentSessionQuestion
{
    public long Id { get; set; }

    public long AssessmentSessionId { get; set; }

    public long QuestionId { get; set; }

    public string? SelectedAnswer { get; set; }

    public bool? Result { get; set; }

    public int TimeToComplete { get; set; }

    public virtual AssessmentSession AssessmentSession { get; set; } = null!;

    public virtual QuestionBank Question { get; set; } = null!;
}

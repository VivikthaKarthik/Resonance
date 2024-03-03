using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class AssessmentSession
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public string AssessmentType { get; set; } = null!;

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool? Result { get; set; }

    public virtual AssessmentSessionQuestion? AssessmentSessionQuestion { get; set; }

    public virtual Student IdNavigation { get; set; } = null!;
}

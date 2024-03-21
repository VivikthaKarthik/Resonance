using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class AssessmentLevel
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<AssessmentSession> AssessmentSessions { get; } = new List<AssessmentSession>();
}

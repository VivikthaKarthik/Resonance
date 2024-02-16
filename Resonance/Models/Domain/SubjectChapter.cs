using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class SubjectChapter
{
    public long Id { get; set; }

    public long ChapterId { get; set; }

    public long SubjectId { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }
}

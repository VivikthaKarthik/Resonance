using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Attachment
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string SourceUrl { get; set; } = null!;

    public long AttachmentTypeId { get; set; }

    public long SubTopicId { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime UpdatedOn { get; set; }

    public virtual AttachmentType AttachmentType { get; set; } = null!;

    public virtual SubTopic SubTopic { get; set; } = null!;
}

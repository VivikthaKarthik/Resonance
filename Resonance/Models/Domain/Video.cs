using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Video
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ThumbNail { get; set; } = null!;

    public string SourceUrl { get; set; } = null!;

    public long? SubTopicId { get; set; }

    public long? TopicId { get; set; }

    public long? ChapterId { get; set; }

    public bool? HomeDisplay { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual SubTopic? SubTopic { get; set; }

    public virtual Topic? Topic { get; set; }
}

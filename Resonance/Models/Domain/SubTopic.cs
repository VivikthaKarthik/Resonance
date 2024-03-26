using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class SubTopic
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public string SourceUrl { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool HomeDisplay { get; set; }

    public int? Duration { get; set; }

    public long? TopicId { get; set; }

    public long ChapterId { get; set; }

    public string? ClassNotesUrl { get; set; }

    public string? ExtractUrl { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual ICollection<Attachment> Attachments { get; } = new List<Attachment>();

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestions { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<QuestionBank> QuestionBanks { get; } = new List<QuestionBank>();

    public virtual Topic? Topic { get; set; }

    public virtual ICollection<Video> Videos { get; } = new List<Video>();
}

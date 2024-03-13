using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class SubTopic
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long TopicId { get; set; }

    public string Thumbnail { get; set; } = null!;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual Topic IdNavigation { get; set; } = null!;

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestions { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<QuestionBank> QuestionBanks { get; } = new List<QuestionBank>();

    public virtual ICollection<Video> Videos { get; } = new List<Video>();
}

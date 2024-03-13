using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Topic
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long ChapterId { get; set; }

    public string Thumbnail { get; set; } = null!;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public string? Description { get; set; }

    public virtual Chapter Chapter { get; set; } = null!;

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestions { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<QuestionBank> QuestionBanks { get; } = new List<QuestionBank>();

    public virtual ICollection<Question> Questions { get; } = new List<Question>();

    public virtual SubTopic? SubTopic { get; set; }

    public virtual ICollection<Video> Videos { get; } = new List<Video>();
}

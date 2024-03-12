using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Question
{
    public long Id { get; set; }

    public string? Text { get; set; }

    public bool HasImage { get; set; }

    public long FirstChoiceId { get; set; }

    public long SecondChoiceId { get; set; }

    public long ThirdChoiceId { get; set; }

    public long FourthChoiceId { get; set; }

    public long CorrectChoiceId { get; set; }

    public string? Explanation { get; set; }

    public long? DifficultyLevelId { get; set; }

    public long? SubTopicId { get; set; }

    public long? TopicId { get; set; }

    public long? ChapterId { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual Choice CorrectChoice { get; set; } = null!;

    public virtual Choice FirstChoice { get; set; } = null!;

    public virtual Choice FourthChoice { get; set; } = null!;

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestions { get; } = new List<MultipleChoiceQuestion>();

    public virtual Choice SecondChoice { get; set; } = null!;

    public virtual Choice ThirdChoice { get; set; } = null!;

    public virtual Topic? Topic { get; set; }
}

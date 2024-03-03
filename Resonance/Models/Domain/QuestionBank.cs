using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class QuestionBank
{
    public long Id { get; set; }

    public string Question { get; set; } = null!;

    public string FirstAnswer { get; set; } = null!;

    public string SecondAnswer { get; set; } = null!;

    public string ThirdAnswer { get; set; } = null!;

    public string FourthAnswer { get; set; } = null!;

    public string CorrectAnswer { get; set; } = null!;

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

    public virtual AssessmentSessionQuestion? AssessmentSessionQuestion { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual SubTopic? SubTopic { get; set; }

    public virtual Topic? Topic { get; set; }
}

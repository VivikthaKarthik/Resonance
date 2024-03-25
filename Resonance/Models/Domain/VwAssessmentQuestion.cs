using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class VwAssessmentQuestion
{
    public long Id { get; set; }

    public string Question { get; set; } = null!;

    public string FirstAnswer { get; set; } = null!;

    public string SecondAnswer { get; set; } = null!;

    public string ThirdAnswer { get; set; } = null!;

    public string FourthAnswer { get; set; } = null!;

    public string CorrectAnswer { get; set; } = null!;

    public string DifficultyLevel { get; set; } = null!;

    public bool? IsPreviousYearQuestion { get; set; }

    public long? SubTopicId { get; set; }

    public long? TopicId { get; set; }

    public long? ChapterId { get; set; }

    public bool? Result { get; set; }
}

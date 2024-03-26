using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class VwSessionResult
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public long AssessmentSessionId { get; set; }

    public string AssessmentType { get; set; } = null!;

    public string CorrectAnswer { get; set; } = null!;

    public long? DifficultyLevelId { get; set; }

    public string DifficultyLevel { get; set; } = null!;

    public long AssessmentLevelId { get; set; }

    public string AssessmentLevel { get; set; } = null!;

    public bool? Result { get; set; }

    public string? SelectedAnswer { get; set; }

    public int TimeToComplete { get; set; }

    public bool? IsPreviousYearQuestion { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public long? SubTopicId { get; set; }

    public long? TopicId { get; set; }

    public long? ChapterId { get; set; }
}

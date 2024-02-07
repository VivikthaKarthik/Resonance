using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Choice
{
    public long Id { get; set; }

    public string? Text { get; set; }

    public string? Image { get; set; }

    public bool IsImage { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual ICollection<ExamResult> ExamResults { get; } = new List<ExamResult>();

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestionChoice1s { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestionChoice2s { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestionChoice3s { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestionChoice4s { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestionCorrectChoices { get; } = new List<MultipleChoiceQuestion>();
}

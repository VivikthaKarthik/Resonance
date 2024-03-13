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

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestionCorrectChoices { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestionFirstChoices { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestionFourthChoices { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestionSecondChoices { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestionThirdChoices { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<Question> QuestionCorrectChoices { get; } = new List<Question>();

    public virtual ICollection<Question> QuestionFirstChoices { get; } = new List<Question>();

    public virtual ICollection<Question> QuestionFourthChoices { get; } = new List<Question>();

    public virtual ICollection<Question> QuestionSecondChoices { get; } = new List<Question>();

    public virtual ICollection<Question> QuestionThirdChoices { get; } = new List<Question>();
}

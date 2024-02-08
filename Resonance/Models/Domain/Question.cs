using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Question
{
    public long Id { get; set; }

    public string? Text { get; set; }

    public string? Image { get; set; }

    public bool HasImage { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual ICollection<ExamResult> ExamResults { get; } = new List<ExamResult>();

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestions { get; } = new List<MultipleChoiceQuestion>();
}

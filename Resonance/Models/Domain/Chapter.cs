using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Chapter
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public long SubjectId { get; set; }

    public string Thumbnail { get; set; } = null!;

    public bool IsRecommended { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual ICollection<ExamResult> ExamResults { get; } = new List<ExamResult>();

    public virtual ICollection<MultipleChoiceQuestion> MultipleChoiceQuestions { get; } = new List<MultipleChoiceQuestion>();

    public virtual ICollection<Question> Questions { get; } = new List<Question>();

    public virtual ICollection<Video> Videos { get; } = new List<Video>();
}

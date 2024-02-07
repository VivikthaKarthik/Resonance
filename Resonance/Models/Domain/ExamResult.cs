using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class ExamResult
{
    public long Id { get; set; }

    public long StudentId { get; set; }

    public long ExamId { get; set; }

    public DateTime ConductedOn { get; set; }

    public string ExamType { get; set; } = null!;

    public long QuestionId { get; set; }

    public long AnswerId { get; set; }

    public string Status { get; set; } = null!;

    public long? SubjectId { get; set; }

    public long? ChapterId { get; set; }

    public long? TopicId { get; set; }

    public long? SubTopicId { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual Choice Answer { get; set; } = null!;

    public virtual Chapter? Chapter { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual SubTopic? SubTopic { get; set; }

    public virtual Subject? Subject { get; set; }

    public virtual Topic? Topic { get; set; }
}

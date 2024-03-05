using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class QuestionImage
{
    public long Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public long QuestionId { get; set; }

    public int ImageOrder { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual Question Question { get; set; } = null!;
}

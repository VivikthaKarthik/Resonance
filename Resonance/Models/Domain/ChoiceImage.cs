using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class ChoiceImage
{
    public long Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public long ChoiceId { get; set; }

    public int ImageOrder { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual Choice Choice { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class City
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long StateId { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual State State { get; set; } = null!;

    public virtual ICollection<Student> Students { get; } = new List<Student>();
}

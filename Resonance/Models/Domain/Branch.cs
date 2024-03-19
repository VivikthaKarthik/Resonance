using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Branch
{
    public long Id { get; set; }

    public string BranchId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string LicenceKey { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Student> Students { get; } = new List<Student>();

    public virtual ICollection<User> Users { get; } = new List<User>();
}

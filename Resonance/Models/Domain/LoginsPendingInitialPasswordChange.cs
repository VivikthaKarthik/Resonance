using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class LoginsPendingInitialPasswordChange
{
    public string Name { get; set; } = null!;

    public byte[] Sid { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }
}

using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class LoginModification
{
    public int Id { get; set; }

    public string EventData { get; set; } = null!;

    public string? ExtraInfo { get; set; }

    public string EventType { get; set; } = null!;

    public DateTime ExecutedAt { get; set; }

    public string ChangeStatus { get; set; } = null!;
}

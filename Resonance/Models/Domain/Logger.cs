using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class Logger
{
    public int Id { get; set; }

    public string ReferenceNumber { get; set; } = null!;

    public string LogType { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? StackTrace { get; set; }

    public string EntityName { get; set; } = null!;

    public string? ExceptionType { get; set; }

    public DateTime CreateOn { get; set; }
}

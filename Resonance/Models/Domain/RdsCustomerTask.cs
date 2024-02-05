using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class RdsCustomerTask
{
    public int TaskId { get; set; }

    public string TaskType { get; set; } = null!;

    public string Lifecycle { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime LastUpdated { get; set; }

    public string ServerName { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string? NewDatabaseName { get; set; }

    public int? DatabaseId { get; set; }

    public Guid? FamilyGuid { get; set; }

    public string? S3ObjectArn { get; set; }

    public bool? OverwriteS3BackupFile { get; set; }

    public string? KmsMasterKeyArn { get; set; }

    public int? TaskProgress { get; set; }

    public bool? Cvc { get; set; }

    public string? TaskInfo { get; set; }

    public string? Filepath { get; set; }

    public bool OverwriteFile { get; set; }

    public string? TaskMetadata { get; set; }
}

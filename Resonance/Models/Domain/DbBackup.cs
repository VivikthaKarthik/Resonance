using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class DbBackup
{
    public Guid FamilyGuid { get; set; }

    public long BeforeBackupEpoch { get; set; }

    public long AfterBackupEpoch { get; set; }

    public string DatabaseName { get; set; } = null!;

    public int? OptionalChecksum { get; set; }

    public byte[]? DatabaseBackup { get; set; }

    public string? DatabaseBackupS3BucketName { get; set; }

    public string? DatabaseBackupS3Key { get; set; }
}

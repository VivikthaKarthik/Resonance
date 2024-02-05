using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class LogBackupManifest
{
    public Guid FamilyGuid { get; set; }

    public int RdsSequenceId { get; set; }

    public int BackupRoundId { get; set; }

    public long FileEpoch { get; set; }

    public DateTime? BackupFileTime { get; set; }

    public string DatabaseName { get; set; } = null!;

    public string Lifecycle { get; set; } = null!;

    public long? FileSizeBytes { get; set; }

    public decimal? FirstLsn { get; set; }

    public decimal? LastLsn { get; set; }

    public bool? IsLogChainBroken { get; set; }

    public string DatabaseState { get; set; } = null!;

    public string DatabaseRecoveryModel { get; set; } = null!;

    public string? S3MetadataFileContent { get; set; }
}

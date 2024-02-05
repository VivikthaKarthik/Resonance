using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class RdsSystemDatabaseObjectSyncInfo
{
    public string ObjectClass { get; set; } = null!;

    public long? ObjectHash { get; set; }

    public string SyncLifecycle { get; set; } = null!;

    public DateTime LastInSyncTime { get; set; }
}

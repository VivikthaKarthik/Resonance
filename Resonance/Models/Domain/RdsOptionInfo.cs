using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class RdsOptionInfo
{
    public string OptionName { get; set; } = null!;

    public string MajorEngineVersion { get; set; } = null!;

    public int? Port { get; set; }

    public string Lifecycle { get; set; } = null!;

    public string ChangeState { get; set; } = null!;

    public long? InstallStartEpoch { get; set; }

    public long? InstallEndEpoch { get; set; }

    public virtual ICollection<RdsOptionSettingsInfo> RdsOptionSettingsInfos { get; } = new List<RdsOptionSettingsInfo>();
}

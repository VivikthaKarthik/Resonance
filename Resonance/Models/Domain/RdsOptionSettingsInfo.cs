using System;
using System.Collections.Generic;

namespace ResoClassAPI.Models.Domain;

public partial class RdsOptionSettingsInfo
{
    public string OptionName { get; set; } = null!;

    public string OptionSetting { get; set; } = null!;

    public string OptionSettingValue { get; set; } = null!;

    public virtual RdsOptionInfo OptionNameNavigation { get; set; } = null!;
}

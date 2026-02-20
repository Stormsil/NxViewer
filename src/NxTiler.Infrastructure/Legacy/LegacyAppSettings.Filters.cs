using System.Configuration;

namespace NxTiler.Infrastructure.Legacy;

internal sealed partial class LegacyAppSettings
{
    [UserScopedSetting]
    [DefaultSettingValue("")]
    public string TitleFilter
    {
        get => (string)this[nameof(TitleFilter)];
        set => this[nameof(TitleFilter)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("^(WoW|Poe)\\d+$")]
    public string NameFilter
    {
        get => (string)this[nameof(NameFilter)];
        set => this[nameof(NameFilter)] = value;
    }

    [UserScopedSetting]
    public string NxsFolder
    {
        get
        {
            var value = (string)this[nameof(NxsFolder)];
            return string.IsNullOrEmpty(value)
                ? Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Documents\NoMachine")
                : value;
        }
        set => this[nameof(NxsFolder)] = value;
    }
}

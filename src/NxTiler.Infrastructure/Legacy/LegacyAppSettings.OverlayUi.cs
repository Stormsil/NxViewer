using System.Configuration;

namespace NxTiler.Infrastructure.Legacy;

internal sealed partial class LegacyAppSettings
{
    [UserScopedSetting]
    [DefaultSettingValue("-1")]
    public double OverlayLeft
    {
        get => (double)this[nameof(OverlayLeft)];
        set => this[nameof(OverlayLeft)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("-1")]
    public double OverlayTop
    {
        get => (double)this[nameof(OverlayTop)];
        set => this[nameof(OverlayTop)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("false")]
    public bool TrayHintShown
    {
        get => (bool)this[nameof(TrayHintShown)];
        set => this[nameof(TrayHintShown)] = value;
    }
}

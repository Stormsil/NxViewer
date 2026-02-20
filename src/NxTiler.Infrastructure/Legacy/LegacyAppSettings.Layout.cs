using System.Configuration;

namespace NxTiler.Infrastructure.Legacy;

internal sealed partial class LegacyAppSettings
{
    [UserScopedSetting]
    [DefaultSettingValue("1")]
    public int Gap
    {
        get => (int)this[nameof(Gap)];
        set => this[nameof(Gap)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("28")]
    public int TopPad
    {
        get => (int)this[nameof(TopPad)];
        set => this[nameof(TopPad)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("false")]
    public bool SortDesc
    {
        get => (bool)this[nameof(SortDesc)];
        set => this[nameof(SortDesc)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("true")]
    public bool SuspendOnMax
    {
        get => (bool)this[nameof(SuspendOnMax)];
        set => this[nameof(SuspendOnMax)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("1500")]
    public int DragCooldownMs
    {
        get => (int)this[nameof(DragCooldownMs)];
        set => this[nameof(DragCooldownMs)] = value;
    }
}

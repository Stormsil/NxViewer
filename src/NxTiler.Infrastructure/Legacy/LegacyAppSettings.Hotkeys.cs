using System.Configuration;

namespace NxTiler.Infrastructure.Legacy;

internal sealed partial class LegacyAppSettings
{
    [UserScopedSetting]
    [DefaultSettingValue("112")]
    public int HkOverlayKey { get => (int)this[nameof(HkOverlayKey)]; set => this[nameof(HkOverlayKey)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("0")]
    public uint HkOverlayMod { get => (uint)this[nameof(HkOverlayMod)]; set => this[nameof(HkOverlayMod)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("112")]
    public int HkMainKey { get => (int)this[nameof(HkMainKey)]; set => this[nameof(HkMainKey)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("2")]
    public uint HkMainMod { get => (uint)this[nameof(HkMainMod)]; set => this[nameof(HkMainMod)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("112")]
    public int HkFocusKey { get => (int)this[nameof(HkFocusKey)]; set => this[nameof(HkFocusKey)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("4")]
    public uint HkFocusMod { get => (uint)this[nameof(HkFocusMod)]; set => this[nameof(HkFocusMod)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("192")]
    public int HkMinimizeKey { get => (int)this[nameof(HkMinimizeKey)]; set => this[nameof(HkMinimizeKey)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("0")]
    public uint HkMinimizeMod { get => (uint)this[nameof(HkMinimizeMod)]; set => this[nameof(HkMinimizeMod)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("37")]
    public int HkPrevKey { get => (int)this[nameof(HkPrevKey)]; set => this[nameof(HkPrevKey)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("0")]
    public uint HkPrevMod { get => (uint)this[nameof(HkPrevMod)]; set => this[nameof(HkPrevMod)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("39")]
    public int HkNextKey { get => (int)this[nameof(HkNextKey)]; set => this[nameof(HkNextKey)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("0")]
    public uint HkNextMod { get => (uint)this[nameof(HkNextMod)]; set => this[nameof(HkNextMod)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("113")]
    public int HkRecordKey { get => (int)this[nameof(HkRecordKey)]; set => this[nameof(HkRecordKey)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("0")]
    public uint HkRecordMod { get => (uint)this[nameof(HkRecordMod)]; set => this[nameof(HkRecordMod)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("114")]
    public int HkRecPauseKey { get => (int)this[nameof(HkRecPauseKey)]; set => this[nameof(HkRecPauseKey)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("0")]
    public uint HkRecPauseMod { get => (uint)this[nameof(HkRecPauseMod)]; set => this[nameof(HkRecPauseMod)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("115")]
    public int HkRecStopKey { get => (int)this[nameof(HkRecStopKey)]; set => this[nameof(HkRecStopKey)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("0")]
    public uint HkRecStopMod { get => (uint)this[nameof(HkRecStopMod)]; set => this[nameof(HkRecStopMod)] = value; }
}

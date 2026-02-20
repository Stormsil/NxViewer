using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Windows.Input; // For Key enum if needed, or just store int

namespace NxTiler
{
    public class AppSettings : ApplicationSettingsBase
    {
        private static AppSettings _defaultInstance = ((AppSettings)(Synchronized(new AppSettings())));

        public static AppSettings Default => _defaultInstance;

        // ... existing properties ...
        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string TitleFilter
        {
            get => (string)this["TitleFilter"];
            set => this["TitleFilter"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("^(WoW|Poe)\\d+$")]
        public string NameFilter
        {
            get => (string)this["NameFilter"];
            set => this["NameFilter"] = value;
        }

        [UserScopedSetting]
        public string NxsFolder
        {
            get 
            {
                var val = (string)this["NxsFolder"];
                if (string.IsNullOrEmpty(val))
                    return Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Documents\NoMachine");
                return val;
            }
            set => this["NxsFolder"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("1")]
        public int Gap
        {
            get => (int)this["Gap"];
            set => this["Gap"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("28")]
        public int TopPad
        {
            get => (int)this["TopPad"];
            set => this["TopPad"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool SortDesc
        {
            get => (bool)this["SortDesc"];
            set => this["SortDesc"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("true")]
        public bool SuspendOnMax
        {
            get => (bool)this["SuspendOnMax"];
            set => this["SuspendOnMax"] = value;
        }

        [UserScopedSetting]
        public StringCollection DisabledFiles
        {
            get
            {
                var collection = (StringCollection)this["DisabledFiles"];
                if (collection == null)
                {
                    collection = new StringCollection();
                    this["DisabledFiles"] = collection;
                }
                return collection;
            }
            set => this["DisabledFiles"] = value;
        }

        // ===== SMART AUTO =====
        [UserScopedSetting]
        [DefaultSettingValue("1500")]
        public int DragCooldownMs { get => (int)this["DragCooldownMs"]; set => this["DragCooldownMs"] = value; }

        // ===== OVERLAY POSITION =====
        [UserScopedSetting]
        [DefaultSettingValue("-1")]
        public double OverlayLeft { get => (double)this["OverlayLeft"]; set => this["OverlayLeft"] = value; }

        [UserScopedSetting]
        [DefaultSettingValue("-1")]
        public double OverlayTop { get => (double)this["OverlayTop"]; set => this["OverlayTop"] = value; }

        // ===== TRAY HINT =====
        [UserScopedSetting]
        [DefaultSettingValue("false")]
        public bool TrayHintShown { get => (bool)this["TrayHintShown"]; set => this["TrayHintShown"] = value; }

        // ===== HOTKEYS =====
        // Storing as int (Virtual Key Code) + Modifiers (uint)
        
        // F1 (0x70 = 112)
        [UserScopedSetting]
        [DefaultSettingValue("112")] 
        public int HkOverlayKey { get => (int)this["HkOverlayKey"]; set => this["HkOverlayKey"] = value; }
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public uint HkOverlayMod { get => (uint)this["HkOverlayMod"]; set => this["HkOverlayMod"] = value; }

        // Ctrl + F1
        [UserScopedSetting]
        [DefaultSettingValue("112")] 
        public int HkMainKey { get => (int)this["HkMainKey"]; set => this["HkMainKey"] = value; }
        [UserScopedSetting]
        [DefaultSettingValue("2")] // MOD_CONTROL
        public uint HkMainMod { get => (uint)this["HkMainMod"]; set => this["HkMainMod"] = value; }

        // Shift + F1
        [UserScopedSetting]
        [DefaultSettingValue("112")]
        public int HkFocusKey { get => (int)this["HkFocusKey"]; set => this["HkFocusKey"] = value; }
        [UserScopedSetting]
        [DefaultSettingValue("4")] // MOD_SHIFT
        public uint HkFocusMod { get => (uint)this["HkFocusMod"]; set => this["HkFocusMod"] = value; }

        // ~ (0xC0 = 192)
        [UserScopedSetting]
        [DefaultSettingValue("192")]
        public int HkMinimizeKey { get => (int)this["HkMinimizeKey"]; set => this["HkMinimizeKey"] = value; }
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public uint HkMinimizeMod { get => (uint)this["HkMinimizeMod"]; set => this["HkMinimizeMod"] = value; }

        // Navigation: Previous window (Left Arrow = 0x25 = 37)
        [UserScopedSetting]
        [DefaultSettingValue("37")]
        public int HkPrevKey { get => (int)this["HkPrevKey"]; set => this["HkPrevKey"] = value; }
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public uint HkPrevMod { get => (uint)this["HkPrevMod"]; set => this["HkPrevMod"] = value; }

        // Navigation: Next window (Right Arrow = 0x27 = 39)
        [UserScopedSetting]
        [DefaultSettingValue("39")]
        public int HkNextKey { get => (int)this["HkNextKey"]; set => this["HkNextKey"] = value; }
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public uint HkNextMod { get => (uint)this["HkNextMod"]; set => this["HkNextMod"] = value; }

        // ===== RECORDING =====

        [UserScopedSetting]
        public string RecordingFolder
        {
            get
            {
                var val = (string)this["RecordingFolder"];
                if (string.IsNullOrEmpty(val))
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                return val;
            }
            set => this["RecordingFolder"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("30")]
        public int RecordingFps { get => (int)this["RecordingFps"]; set => this["RecordingFps"] = value; }

        [UserScopedSetting]
        [DefaultSettingValue("ffmpeg")]
        public string FfmpegPath
        {
            get
            {
                var val = (string)this["FfmpegPath"];
                return string.IsNullOrEmpty(val) ? "ffmpeg" : val;
            }
            set => this["FfmpegPath"] = value;
        }

        // Record hotkey (F2 = 0x71 = 113)
        [UserScopedSetting]
        [DefaultSettingValue("113")]
        public int HkRecordKey { get => (int)this["HkRecordKey"]; set => this["HkRecordKey"] = value; }
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public uint HkRecordMod { get => (uint)this["HkRecordMod"]; set => this["HkRecordMod"] = value; }

        // Pause hotkey (F3 = 0x72 = 114)
        [UserScopedSetting]
        [DefaultSettingValue("114")]
        public int HkRecPauseKey { get => (int)this["HkRecPauseKey"]; set => this["HkRecPauseKey"] = value; }
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public uint HkRecPauseMod { get => (uint)this["HkRecPauseMod"]; set => this["HkRecPauseMod"] = value; }

        // Stop hotkey (F4 = 0x73 = 115)
        [UserScopedSetting]
        [DefaultSettingValue("115")]
        public int HkRecStopKey { get => (int)this["HkRecStopKey"]; set => this["HkRecStopKey"] = value; }
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public uint HkRecStopMod { get => (uint)this["HkRecStopMod"]; set => this["HkRecStopMod"] = value; }
    }
}

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
    }
}

using System;
using System.Configuration;
using System.Collections.Specialized;

namespace NxTiler
{
    public class AppSettings : ApplicationSettingsBase
    {
        private static AppSettings _defaultInstance = ((AppSettings)(Synchronized(new AppSettings())));

        public static AppSettings Default => _defaultInstance;

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
    }
}
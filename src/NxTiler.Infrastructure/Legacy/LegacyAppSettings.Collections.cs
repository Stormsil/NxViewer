using System.Collections.Specialized;
using System.Configuration;

namespace NxTiler.Infrastructure.Legacy;

internal sealed partial class LegacyAppSettings
{
    [UserScopedSetting]
    public StringCollection DisabledFiles
    {
        get
        {
            var collection = (StringCollection)this[nameof(DisabledFiles)];
            if (collection is null)
            {
                collection = new StringCollection();
                this[nameof(DisabledFiles)] = collection;
            }

            return collection;
        }
        set => this[nameof(DisabledFiles)] = value;
    }
}

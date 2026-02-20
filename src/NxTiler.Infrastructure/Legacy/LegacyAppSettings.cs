using System.Configuration;

namespace NxTiler.Infrastructure.Legacy;

internal sealed partial class LegacyAppSettings : ApplicationSettingsBase
{
    private static readonly LegacyAppSettings Instance = (LegacyAppSettings)Synchronized(new LegacyAppSettings());

    public static LegacyAppSettings Default => Instance;
}

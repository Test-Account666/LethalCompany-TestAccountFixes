using LobbyCompatibility.Enums;

namespace TestAccountFixes.Dependencies;

internal static class LobbyCompatibilitySupport {
    internal static void Initialize() =>
        TestAccountCore.Dependencies.LobbyCompatibilitySupport.Initialize(MyPluginInfo.PLUGIN_GUID,
                                                                          MyPluginInfo.PLUGIN_VERSION,
                                                                          CompatibilityLevel.ClientOnly,
                                                                          VersionStrictness.Minor);
}
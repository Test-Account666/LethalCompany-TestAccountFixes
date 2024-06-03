using System.Linq;
using BepInEx.Bootstrap;

namespace TestAccountFixes.Dependencies;

internal static class DependencyChecker {
    internal static bool IsLobbyCompatibilityInstalled() =>
        Chainloader.PluginInfos.Values.Any(metadata => metadata.Metadata.GUID.Contains("LobbyCompatibility"));

    internal static bool IsBetterItemHandlingInstalled() =>
        Chainloader.PluginInfos.Values.Any(metadata =>
                                               metadata.Metadata.GUID.Contains("Yan01h.BetterItemHandling"));

    internal static bool IsTelevisionControllerInstalled() =>
        Chainloader.PluginInfos.Values.Any(metadata =>
                                               metadata.Metadata.GUID.Contains("KoderTech.TelevisionController"));
}
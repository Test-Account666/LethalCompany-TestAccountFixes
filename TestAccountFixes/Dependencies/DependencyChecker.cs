using System.Linq;
using BepInEx.Bootstrap;

namespace TestAccountFixes.Dependencies;

internal static class DependencyChecker {
    internal static bool IsLobbyCompatibilityInstalled() =>
        TestAccountCore.Dependencies.DependencyChecker.IsLobbyCompatibilityInstalled();

    internal static bool IsBetterItemHandlingInstalled() =>
        Chainloader.PluginInfos.Values.Any(metadata =>
                                               metadata.Metadata.GUID.Contains("Yan01h.BetterItemHandling"));

    internal static bool IsTelevisionControllerInstalled() =>
        Chainloader.PluginInfos.Values.Any(metadata =>
                                               metadata.Metadata.GUID.Contains("KoderTech.TelevisionController"));

    internal static bool IsInventoryFixPluginInstalled() =>
        Chainloader.PluginInfos.Values.Any(metadata =>
                                               metadata.Metadata.GUID.Contains("Dokge.InventoryFixPlugin"));
}
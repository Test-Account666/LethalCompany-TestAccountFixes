using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;
using UnityEngine.UIElements.Collections;

namespace TestAccountFixes.Dependencies;

internal static class DependencyChecker {
    private static readonly Dictionary<string, bool> _DependencyDictionary = [
    ];

    internal static bool IsLobbyCompatibilityInstalled() => TestAccountCore.Dependencies.DependencyChecker.IsLobbyCompatibilityInstalled();

    internal static bool IsBetterItemHandlingInstalled() => IsInstalled("Yan01h.BetterItemHandling");

    internal static bool IsTelevisionControllerInstalled() => IsInstalled("KoderTech.TelevisionController");

    internal static bool IsInventoryFixPluginInstalled() => IsInstalled("Dokge.InventoryFixPlugin");

    private static bool IsInstalled(string key) {
        return _DependencyDictionary.ContainsKey(key)
            ? _DependencyDictionary.Get(key)
            : Chainloader.PluginInfos.Values.Any(metadata => metadata.Metadata.GUID.Contains(key));
    }
}
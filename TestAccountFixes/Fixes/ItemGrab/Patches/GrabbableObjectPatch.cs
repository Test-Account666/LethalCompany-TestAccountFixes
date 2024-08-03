using HarmonyLib;
using TestAccountFixes.Core;

namespace TestAccountFixes.Fixes.ItemGrab.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public static class GrabbableObjectPatch {
    [HarmonyPatch(nameof(StartOfRound.Start))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static void SetGrabbableBeforeStart() {
        var allowGrab = ItemGrabFix.Instance.allowItemGrabBeforeGameStart.Value;

        if (!allowGrab) return;

        foreach (var itemProperties in StartOfRound.Instance.allItemsList.itemsList) {
            ItemGrabFix.LogDebug($"{itemProperties.itemName} can be grabbed? {itemProperties.canBeGrabbedBeforeGameStart}", LogLevel.VERBOSE);

            if (itemProperties.canBeGrabbedBeforeGameStart) continue;

            ItemGrabFix.LogDebug($"{itemProperties.itemName} can now be grabbed :)", LogLevel.VERBOSE);

            itemProperties.canBeGrabbedBeforeGameStart = true;
        }
    }
}
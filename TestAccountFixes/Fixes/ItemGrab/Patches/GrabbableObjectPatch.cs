using HarmonyLib;
using TestAccountFixes.Core;

namespace TestAccountFixes.Fixes.ItemGrab.Patches;

[HarmonyPatch(typeof(GrabbableObject))]
public static class GrabbableObjectPatch {
    [HarmonyPatch(nameof(GrabbableObject.Start))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void SetGrabbableBeforeStart(GrabbableObject __instance) {
        var allowGrab = ItemGrabFix.Instance.allowItemGrabBeforeGameStart.Value;

        ItemGrabFix.LogDebug($"Allowing grab? {allowGrab}", LogLevel.VERY_VERBOSE);

        if (!allowGrab) return;

        var itemProperties = __instance.itemProperties;

        ItemGrabFix.LogDebug($"{itemProperties.itemName} can be grabbed? {itemProperties.canBeGrabbedBeforeGameStart}", LogLevel.VERBOSE);

        if (itemProperties.canBeGrabbedBeforeGameStart) return;

        ItemGrabFix.LogDebug($"{itemProperties.itemName} can now be grabbed :)", LogLevel.VERBOSE);

        itemProperties.canBeGrabbedBeforeGameStart = true;
    }
}
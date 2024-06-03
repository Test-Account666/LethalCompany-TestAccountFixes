using GameNetcodeStuff;
using HarmonyLib;

namespace TestAccountFixes.Fixes.SpiderWeb.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
public class PlayerControllerBPatch {
    [HarmonyPatch(nameof(PlayerControllerB.LateUpdate))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void UpdatePostfix(PlayerControllerB __instance) =>
        CobwebChecker.HandleCobweb(__instance);

    [HarmonyPatch(nameof(PlayerControllerB.TeleportPlayer))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void TeleportPlayerPostfix(PlayerControllerB __instance) =>
        CobwebChecker.HandleCobweb(__instance);
}
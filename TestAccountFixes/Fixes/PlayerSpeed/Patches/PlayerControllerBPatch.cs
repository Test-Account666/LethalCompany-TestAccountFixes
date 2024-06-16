using GameNetcodeStuff;
using HarmonyLib;
using TestAccountFixes.Core;

namespace TestAccountFixes.Fixes.PlayerSpeed.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
public static class PlayerControllerBPatch {
    [HarmonyPatch(nameof(PlayerControllerB.Update))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static void FixCriticallyInjuredState(PlayerControllerB __instance) {
        if (!__instance.criticallyInjured) {
            PlayerSpeedFix.LogDebug($"Player {__instance.playerUsername} is not injured, skiping!", LogLevel.VERY_VERBOSE);
            return;
        }

        if (__instance.health < 20) {
            PlayerSpeedFix.LogDebug($"Player {__instance.playerUsername} is not below 20HP!", LogLevel.VERY_VERBOSE);
            return;
        }

        PlayerSpeedFix.LogDebug($"Fixing player {__instance.playerUsername}!");

        __instance.criticallyInjured = false;
    }
}
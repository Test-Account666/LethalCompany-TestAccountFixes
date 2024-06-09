using GameNetcodeStuff;
using HarmonyLib;
using TestAccountFixes.Core;

namespace TestAccountFixes.Fixes.DogSound.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
public static class PlayerControllerBPatch {
    [HarmonyPatch(nameof(PlayerControllerB.PlayJumpAudio))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void AlertDoggosBySprintJumping(PlayerControllerB __instance) {
        if (!DogSoundFix.fixSilentSprint.Value)
            return;

        if (!__instance.IsHost) {
            DogSoundFix.LogDebug("[SilentSprint4] We're not host, skipping...");
            return;
        }

        var sprintChecker = __instance.gameObject.GetComponent<SprintChecker>()
                         ?? __instance.gameObject.AddComponent<SprintChecker>();

        if (sprintChecker.currentState is not SprintChecker.PlayerState.SPRINT) return;

        DogSoundFix.LogDebug($"[SilentSprint4] {__instance.playerUsername} is alerting doggos!");

        var noiseIsInsideClosedShip = __instance.isInHangarShipRoom && StartOfRound.Instance.hangarDoorsClosed;

        RoundManager.Instance.PlayAudibleNoise(__instance.transform.position, 13f, 0.6f,
                                               noiseIsInsideClosedShip: noiseIsInsideClosedShip, noiseID: 8);
    }

    [HarmonyPatch(nameof(PlayerControllerB.Start))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void AddCheckForSilentSprint(PlayerControllerB __instance) {
        if (!DogSoundFix.fixSilentSprint.Value)
            return;

        if (!__instance.IsHost) {
            DogSoundFix.LogDebug("[SilentSprint1] We're not host, skipping...");
            return;
        }

        foreach (var allPlayerScript in StartOfRound.Instance.allPlayerScripts) {
            if (allPlayerScript is null)
                continue;

            _ = allPlayerScript.gameObject.GetComponent<SprintChecker>()
             ?? allPlayerScript.gameObject.AddComponent<SprintChecker>();
        }
    }

    [HarmonyPatch(nameof(PlayerControllerB.Update))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void CheckForSilentSprint(PlayerControllerB __instance) {
        if (!DogSoundFix.fixSilentSprint.Value)
            return;

        if (!__instance.IsHost) {
            DogSoundFix.LogDebug("[SilentSprint2] We're not host, skipping...", LogLevel.VERY_VERBOSE);
            return;
        }

        foreach (var allPlayerScript in StartOfRound.Instance.allPlayerScripts) {
            if (allPlayerScript is null)
                continue;

            var sprintChecker = allPlayerScript.gameObject.GetComponent<SprintChecker>()
                             ?? allPlayerScript.gameObject.AddComponent<SprintChecker>();

            sprintChecker.CheckForRapidStateChange(allPlayerScript);
        }
    }
}
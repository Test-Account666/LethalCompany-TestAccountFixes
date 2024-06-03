using HarmonyLib;
using TestAccountFixes.Core;

namespace TestAccountFixes.Fixes.DogSound.Patches;

[HarmonyPatch(typeof(HUDManager))]
public static class HUDManagerPatch {
    [HarmonyPatch(nameof(HUDManager.AddTextToChatOnServer))]
    [HarmonyPostfix]
    private static void MakeChatLoud(int playerId) {
        if (!DogSoundFix.chatIsLoudActually.Value)
            return;

        if (!StartOfRound.Instance.IsHost && !StartOfRound.Instance.IsServer) {
            DogSoundFix.LogDebug("We're not host, skipping...", LogLevel.VERY_VERBOSE);
            return;
        }

        if (StartOfRound.Instance.inShipPhase) {
            DogSoundFix.LogDebug("We're in ship phase, skipping...", LogLevel.VERY_VERBOSE);
            return;
        }

        if (playerId <= -1) {
            DogSoundFix.LogDebug("Invalid player id, or system message, skipping...", LogLevel.VERBOSE);
            return;
        }

        var player = StartOfRound.Instance.allPlayerScripts[playerId];

        var insideClosedShip = player.isInHangarShipRoom && StartOfRound.Instance.hangarDoorsClosed;

        RoundManager.Instance.PlayAudibleNoise(player.transform.position, 15, .6F, 1, insideClosedShip, 666);

        DogSoundFix.LogDebug("Loud chat message!", LogLevel.VERY_VERBOSE);
    }
}
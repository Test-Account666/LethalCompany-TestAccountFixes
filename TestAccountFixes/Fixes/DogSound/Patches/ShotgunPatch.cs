using HarmonyLib;
using TestAccountFixes.Core;
using UnityEngine;

namespace TestAccountFixes.Fixes.DogSound.Patches;

[HarmonyPatch(typeof(ShotgunItem))]
public static class ShotgunPatch {
    [HarmonyPatch("ShootGunServerRpc")]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void ShootGunServerRpcPostfix(ShotgunItem __instance, Vector3 shotgunPosition) =>
        HandleShotgunNoise(__instance, 50F, 6F, shotgunPosition);

    [HarmonyPatch(nameof(ShotgunItem.ItemInteractLeftRight))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void ItemInteractLeftRightPostfix(ShotgunItem __instance) => HandleShotgunNoise(__instance);

    [HarmonyPatch(nameof(ShotgunItem.ReloadGunEffectsServerRpc))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void ReloadGunEffectsServerRpcPostfix(ShotgunItem __instance, bool start) =>
        HandleShotgunNoise(__instance, ignore: start);

    private static void HandleShotgunNoise(ShotgunItem shotgunItem, float noiseRange = 4F, float noiseLoudness = .5F,
                                           Vector3 shotgunPosition = default, bool ignore = false) {
        if (!StartOfRound.Instance.IsHost) {
            DogSoundFix.LogDebug("[Shotgun] We're not host, skipping...", LogLevel.VERBOSE);
            return;
        }

        if (ignore) {
            DogSoundFix.LogDebug("[Shotgun] Ignoring this shot...", LogLevel.VERBOSE);
            return;
        }

        var insideClosedShip = shotgunItem.isInShipRoom && StartOfRound.Instance.hangarDoorsClosed;

#pragma warning disable Harmony003
        if (shotgunPosition == null! || shotgunPosition == default) {
            if (shotgunItem.playerHeldBy is not null)
                shotgunPosition = shotgunItem.playerHeldBy.transform.position;

            if (shotgunItem.heldByEnemy is not null)
                shotgunPosition = shotgunItem.heldByEnemy.transform.position;
        }
#pragma warning restore Harmony003

        RoundManager.Instance.PlayAudibleNoise(shotgunPosition, noiseRange, noiseLoudness, 1, insideClosedShip, 666);

        DogSoundFix.LogDebug("[Shotgun] Alerting doggos!", LogLevel.VERBOSE);
    }
}
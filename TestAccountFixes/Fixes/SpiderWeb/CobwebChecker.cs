using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using TestAccountCore;
using TestAccountFixes.Core;
using UnityEngine;

namespace TestAccountFixes.Fixes.SpiderWeb;

public static class CobwebChecker {
    private static long _nextCheck;

    public static void HandleCobweb(PlayerControllerB playerControllerB) {
        var localPlayer = StartOfRound.Instance.localPlayerController;

        if (playerControllerB != localPlayer) {
            SpiderWebFix.LogDebug($"{playerControllerB.playerUsername} is not local player!", LogLevel.VERY_VERBOSE);
            return;
        }

        var currentTime = UnixTime.GetCurrentTime();

        if (currentTime < _nextCheck) {
            SpiderWebFix.LogDebug($"Still on cooldown, current time '{currentTime}', next check '{_nextCheck}'", LogLevel.VERY_VERBOSE);
            return;
        }

        _nextCheck = currentTime + 1000;

        if (!IsPlayerValid(playerControllerB)) {
            SpiderWebFix.LogDebug($"{playerControllerB.playerUsername} not valid, skipping...", LogLevel.VERY_VERBOSE);
            return;
        }

        var activeCobwebs = FindActiveCobwebs();
        var shouldRelease = ShouldReleaseBasedOnCobwebs(activeCobwebs);

        if (!shouldRelease) {
            SpiderWebFix.LogDebug($"Found active cobwebs for {playerControllerB.playerUsername}, skipping...");
            return;
        }

        if (!ShouldReleaseBasedOnZapGuns()) {
            SpiderWebFix.LogDebug($"Found active zap guns for {playerControllerB.playerUsername}, skipping...");
            return;
        }

        SpiderWebFix.LogDebug($"Releasing {playerControllerB.playerUsername} from pesky cobwebs :>");
        ReleasePlayer(activeCobwebs);
    }

    private static bool IsPlayerValid(PlayerControllerB playerControllerB) {
        if (StartOfRound.Instance is null) {
            SpiderWebFix.LogDebug("StartOfRound is null!", LogLevel.VERBOSE);
            return false;
        }

        var localPlayer = StartOfRound.Instance.localPlayerController;

        if (localPlayer.isSinking) {
            SpiderWebFix.LogDebug($"{playerControllerB.playerUsername} is sinking!", LogLevel.VERBOSE);
            return false;
        }

        if (localPlayer.isMovementHindered > 0) return true;

        SpiderWebFix.LogDebug($"{playerControllerB.playerUsername} is not hindered!", LogLevel.VERY_VERBOSE);
        return false;
    }

    private static List<SandSpiderWebTrap> FindActiveCobwebs() {
        var cobwebs = Object.FindObjectsOfType<SandSpiderWebTrap>();
        cobwebs ??= [
        ];

        return (from cobwebTrap in cobwebs
                where cobwebs is not null
                where cobwebTrap.hinderingLocalPlayer
                select cobwebTrap).ToList();
    }

    private static bool ShouldReleaseBasedOnCobwebs(IReadOnlyCollection<SandSpiderWebTrap> activeCobwebs) {
        if (activeCobwebs.None()) return true;

        var localPlayerPosition = StartOfRound.Instance.localPlayerController.transform.position;
        return (from cobweb in activeCobwebs
                let cobwebPosition = cobweb.transform.position
                let distance = Vector3.Distance(localPlayerPosition, cobwebPosition)
                where !cobweb.webHasBeenBroken && distance < 10F
                select cobweb).None();
    }

    private static bool ShouldReleaseBasedOnZapGuns() {
        List<PatcherTool> zapGuns = [
        ];

        foreach (var playerControllerB in StartOfRound.Instance.allPlayerScripts) {
            foreach (var grabbableObject in playerControllerB.ItemSlots) {
                if (grabbableObject is not PatcherTool patcherTool) continue;

                zapGuns.Add(patcherTool);
            }
        }

        return (from zapGun in zapGuns
                where zapGun is not null
                let shockableTransform = zapGun.shockedTargetScript.GetShockableTransform()
                let shockedPlayer = shockableTransform.GetComponentInChildren<PlayerControllerB>()
                where shockedPlayer is not null && shockedPlayer != StartOfRound.Instance.localPlayerController
                select shockedPlayer).None();
    }

    private static void ReleasePlayer(List<SandSpiderWebTrap> activeCobwebs) {
        var localPlayer = StartOfRound.Instance.localPlayerController;
        localPlayer.isMovementHindered -= activeCobwebs.Count;

        if (localPlayer.isMovementHindered < 0) localPlayer.isMovementHindered = 0;

        foreach (var cobweb in activeCobwebs) {
            cobweb.hinderingLocalPlayer = false;
            cobweb.currentTrappedPlayer = null;

            localPlayer.hinderedMultiplier /= 3.5f;
        }

        if (localPlayer.hinderedMultiplier >= 1) return;

        localPlayer.hinderedMultiplier = 1;
    }
}
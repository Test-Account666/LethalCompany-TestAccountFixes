using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using TestAccountFixes.Core;
using UnityEngine;

namespace TestAccountFixes.Fixes.SpiderWeb;

public static class CobwebChecker {
    public static void HandleCobweb(PlayerControllerB playerControllerB) {
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

        if (playerControllerB != localPlayer) {
            SpiderWebFix.LogDebug($"{playerControllerB.playerUsername} is not local player!", LogLevel.VERY_VERBOSE);
            return false;
        }

        if (localPlayer.isSinking) {
            SpiderWebFix.LogDebug($"{playerControllerB.playerUsername} is sinking!", LogLevel.VERBOSE);
            return false;
        }

        if (localPlayer.isMovementHindered > 0)
            return true;

        SpiderWebFix.LogDebug($"{playerControllerB.playerUsername} is not hindered!", LogLevel.VERY_VERBOSE);
        return false;
    }

    private static List<SandSpiderWebTrap> FindActiveCobwebs() =>
        Object.FindObjectsOfType<SandSpiderWebTrap>()
              .Where(cobwebTrap => cobwebTrap.hinderingLocalPlayer)
              .ToList();

    private static bool ShouldReleaseBasedOnCobwebs(IReadOnlyCollection<SandSpiderWebTrap> activeCobwebs) {
        if (activeCobwebs.None())
            return true;

        var localPlayerPosition = StartOfRound.Instance.localPlayerController.transform.position;
        return (from cobweb in activeCobwebs
                let cobwebPosition = cobweb.transform.position
                let distance = Vector3.Distance(localPlayerPosition, cobwebPosition)
                where !cobweb.webHasBeenBroken && distance < 10F
                select cobweb).None();
    }

    private static bool ShouldReleaseBasedOnZapGuns() {
        var zapGuns = Object.FindObjectsOfType<PatcherTool>();
        return (from zapGun in zapGuns
                where zapGun is not null
                let shockableTransform = zapGun.shockedTargetScript.GetShockableTransform()
                let shockedPlayer = shockableTransform.GetComponentInChildren<PlayerControllerB>()
                where shockedPlayer is not null && shockedPlayer != StartOfRound.Instance.localPlayerController
                select shockedPlayer).None();
    }

    private static void ReleasePlayer(List<SandSpiderWebTrap> activeCobwebs) {
        var localPlayer = StartOfRound.Instance.localPlayerController;
        localPlayer.isMovementHindered = 0;
        localPlayer.hinderedMultiplier /= 3.5f;

        foreach (var cobweb in activeCobwebs) {
            cobweb.hinderingLocalPlayer = false;
            cobweb.currentTrappedPlayer = null;
        }
    }
}
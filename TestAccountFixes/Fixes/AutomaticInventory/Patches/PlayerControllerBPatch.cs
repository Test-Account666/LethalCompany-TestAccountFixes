using System;
using GameNetcodeStuff;
using HarmonyLib;
using MonoMod.Utils;
using TestAccountCore;
using Unity.Netcode;
using UnityEngine;

namespace TestAccountFixes.Fixes.AutomaticInventory.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
public static class PlayerControllerBPatch {
    private static long _throwTimeoutTime;
    private static long _grabTimeoutTime;
    private static NetworkObject? _thrownObject;
    private static readonly int _CancelHoldingHash = Animator.StringToHash("cancelHolding");

    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void AfterUpdate(PlayerControllerB __instance) {
        if (!__instance.isPlayerControlled)
            return;

        if (__instance.isPlayerDead)
            return;

        try {
            ThrowObjectCheck(__instance);
        } catch (Exception exception) {
            exception.LogDetailed();
        }

        try {
            GrabObjectCheck(__instance);
        } catch (Exception exception) {
            exception.LogDetailed();
        }
    }

    [HarmonyPatch(nameof(PlayerControllerB.DiscardHeldObject))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static void DiscardHeldObjectPrefix(PlayerControllerB __instance, NetworkObject parentObjectTo) {
        if (__instance is not {
                IsOwner: true,
            }) return;

        var heldObjectServer = __instance.currentlyHeldObjectServer;
        var heldObjectServerNetworkObject = heldObjectServer.GetComponent<NetworkObject>();

        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        var thrownObject = parentObjectTo ?? heldObjectServerNetworkObject;

        if (thrownObject is null)
            return;

        var currentTime = UnixTime.GetCurrentTime();
        AutomaticInventoryFix.LogDebug("Detected throw! Setting timeout...");
        _throwTimeoutTime = currentTime + 1000;
        _thrownObject = thrownObject;
    }

    private static void ThrowObjectCheck(PlayerControllerB player) {
        if (_throwTimeoutTime <= 0)
            return;

        if (player.hasThrownObject) {
            _throwTimeoutTime = 0;
            return;
        }

        var currentTime = UnixTime.GetCurrentTime();

        if (_throwTimeoutTime > currentTime)
            return;

        AutomaticInventoryFix.LogDebug("Detected throw lasting longer than 1 second, giving up...");

        player.throwingObject = false;
        player.playerBodyAnimator.SetBool(_CancelHoldingHash, false);
        _throwTimeoutTime = 0;

        if (_thrownObject is null)
            return;

        player.GrabObjectServerRpc(_thrownObject);
    }

    private static void GrabObjectCheck(PlayerControllerB player) {
        var currentlyGrabbingObject = player.currentlyGrabbingObject;

        if (currentlyGrabbingObject is null) {
            _grabTimeoutTime = 0;
            return;
        }

        if (player.grabbedObjectValidated) {
            _grabTimeoutTime = 0;
            return;
        }

        var currentTime = UnixTime.GetCurrentTime();

        if (_grabTimeoutTime <= 0) {
            AutomaticInventoryFix.LogDebug("Detected grab! Setting timeout...");
            _grabTimeoutTime = currentTime + 1000;
            return;
        }

        if (_grabTimeoutTime > currentTime)
            return;

        AutomaticInventoryFix.LogDebug("Detected grab lasting longer than 1 second, giving up...");

        player.grabInvalidated = true;

        player.currentlyGrabbingObject = null;

        if (currentlyGrabbingObject.parentObject != player.localItemHolder) return;

        if (currentlyGrabbingObject.playerHeldBy is not null) {
            currentlyGrabbingObject.parentObject = currentlyGrabbingObject.playerHeldBy.serverItemHolder;
            return;
        }

        if (currentlyGrabbingObject.isInShipRoom || currentlyGrabbingObject.isInElevator) {
            currentlyGrabbingObject.parentObject = StartOfRound.Instance.elevatorTransform;
            return;
        }

        currentlyGrabbingObject.parentObject = null;
    }
}
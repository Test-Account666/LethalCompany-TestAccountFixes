using System;
using GameNetcodeStuff;
using HarmonyLib;
using MonoMod.Utils;
using TestAccountFixes.Core;
using UnityEngine;

namespace TestAccountFixes.Fixes.AutomaticInventory.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
public static class PlayerControllerBFinalizer {
    [HarmonyPatch(nameof(PlayerControllerB.SetObjectAsNoLongerHeld))]
    [HarmonyFinalizer]
    // ReSharper disable once InconsistentNaming
    public static Exception
        SetObjectAsNoLongerHeldFinalizer(Exception __exception, GrabbableObject dropObject, Vector3 targetFloorPosition) {
        AutomaticInventoryFix.LogDebug("[NoLongerHeld] Exception? " + (__exception != null!), LogLevel.VERY_VERBOSE);

        if (__exception == null!)
            return null!;


        AutomaticInventoryFix.LogDebug("Catched Exception from PlayerControllerB#SetObjectAsNoLongerHeld method!");

        __exception.LogDetailed();

        AutomaticInventoryFix.LogDebug("More Information:");

        AutomaticInventoryFix.LogDebug("dropObject null? " + (dropObject == null));
        AutomaticInventoryFix.LogDebug("targetFloorPosition null? " + (targetFloorPosition == null!));

        if (dropObject == null)
            return null!;

        AutomaticInventoryFix.LogDebug("transform null? " + (dropObject.transform == null!));

        if (dropObject.transform == null)
            return null!;

        AutomaticInventoryFix.LogDebug("transform position null? " + (dropObject.transform.position == null!));

        AutomaticInventoryFix.LogDebug("transform parent null? " + (dropObject.transform.parent == null!));
        return null!;
    }
}
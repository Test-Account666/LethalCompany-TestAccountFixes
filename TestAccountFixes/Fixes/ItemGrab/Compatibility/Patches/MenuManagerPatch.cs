using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using HarmonyLib;

namespace TestAccountFixes.Fixes.ItemGrab.Compatibility.Patches;

[HarmonyPatch(typeof(MenuManager))]
public static class MenuManagerPatch {
    [HarmonyPatch(nameof(MenuManager.Start))]
    [HarmonyPostfix]
    public static void CheckForGrabObjectPatches() {
        var patches = Harmony.GetPatchInfo(AccessTools.DeclaredMethod(typeof(PlayerControllerB),
                                                                      nameof(PlayerControllerB.BeginGrabObject)));

        if (patches == null)
            return;

        var allPatches = new List<Patch>();

        allPatches.AddRange(patches.Finalizers ?? Enumerable.Empty<Patch>());
        allPatches.AddRange(patches.Postfixes ?? Enumerable.Empty<Patch>());
        allPatches.AddRange(patches.Prefixes ?? Enumerable.Empty<Patch>());
        allPatches.AddRange(patches.Transpilers ?? Enumerable.Empty<Patch>());
        allPatches.AddRange(patches.ILManipulators ?? Enumerable.Empty<Patch>());

        if (allPatches.Count <= 0)
            return;

        TestAccountFixes.Logger.LogWarning("[GrabItemFix] Detected mods patching the PlayerControllerB#BeginGrabObject method!");
        TestAccountFixes.Logger.LogWarning("[GrabItemFix] These mods using may not work correctly!");
        TestAccountFixes.Logger.LogWarning("[GrabItemFix] Please report any issues!");


        HashSet<string> patchOwnerSet = [
        ];

        foreach (var allPatch in allPatches)
            patchOwnerSet.Add(allPatch.owner);

        TestAccountFixes.Logger.LogWarning("[GrabItemFix] Mods that might not work as expected:");

        foreach (var patchOwner in patchOwnerSet)
            TestAccountFixes.Logger.LogWarning("[GrabItemFix] " + patchOwner);
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GameNetcodeStuff;
using HarmonyLib;

namespace TestAccountFixes.Fixes.ItemGrab.Compatibility.Patches;

[HarmonyPatch(typeof(MenuManager))]
public static class MenuManagerPatch {
    [HarmonyPatch(nameof(MenuManager.Start))]
    [HarmonyPostfix]
    public static void CheckForGrabObjectPatches() {
        var method1 = AccessTools.DeclaredMethod(typeof(PlayerControllerB), nameof(PlayerControllerB.BeginGrabObject));

        CheckForPatches(method1);

        var method2 = AccessTools.DeclaredMethod(typeof(PlayerControllerB), nameof(PlayerControllerB.SetHoverTipAndCurrentInteractTrigger));

        CheckForPatches(method2);
    }

    private static void CheckForPatches(MethodInfo methodInfo) {
        var patches = Harmony.GetPatchInfo(methodInfo);

        if (patches == null) return;

        var allPatches = new List<Patch>();

        allPatches.AddRange(patches.Finalizers ?? Enumerable.Empty<Patch>());
        allPatches.AddRange(patches.Postfixes ?? Enumerable.Empty<Patch>());
        allPatches.AddRange(patches.Prefixes ?? Enumerable.Empty<Patch>());
        allPatches.AddRange(patches.Transpilers ?? Enumerable.Empty<Patch>());
        allPatches.AddRange(patches.ILManipulators ?? Enumerable.Empty<Patch>());

        allPatches.RemoveAll(patch => {
            var patchOwner = patch?.owner;
            return patchOwner is null || patchOwner.StartsWith("TestAccount666.TestAccountFixes");
        });

        if (allPatches.Count <= 0) return;

        TestAccountFixes.Logger.LogWarning(new StringBuilder()
                                           .Append("[GrabItemFix] Detected mods patching the ")
                                           .Append(methodInfo.DeclaringType?.FullName ?? "null")
                                           .Append("#")
                                           .Append(methodInfo.Name)
                                           .Append(" method!"));
        TestAccountFixes.Logger.LogWarning("[GrabItemFix] This could be an issue!");
        TestAccountFixes.Logger.LogWarning("[GrabItemFix] Please report any issues!");


        HashSet<string> patchOwnerSet = [
        ];

        foreach (var allPatch in allPatches) patchOwnerSet.Add(allPatch.owner);

        TestAccountFixes.Logger.LogWarning("[GrabItemFix] Mods that might not work as expected:");

        TestAccountFixes.Logger.LogWarning("[GrabItemFix] ~~~~ Start of list ~~~~");

        foreach (var patchOwner in patchOwnerSet) TestAccountFixes.Logger.LogWarning("[GrabItemFix] " + patchOwner);

        TestAccountFixes.Logger.LogWarning("[GrabItemFix] ~~~~ End of list ~~~~");

        TestAccountFixes.Logger.LogWarning("[GrabItemFix] Keep in mind... "
                                         + "Just because a mod is listed here, doesn't mean it will cause issues!");
    }
}
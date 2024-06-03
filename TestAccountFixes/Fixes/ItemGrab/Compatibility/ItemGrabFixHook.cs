using GameNetcodeStuff;
using TestAccountFixes.Core;

namespace TestAccountFixes.Fixes.ItemGrab.Compatibility;

public static class ItemGrabFixHook {
    public delegate void GrabObjectEvent(GrabObjectEventArgs args);

    internal static void OnPreBeforeGrabObject(PlayerControllerB playerControllerB, GrabbableObject? grabbableObject) {
        ItemGrabFix.LogDebug("OnPreBeforeGrabObject!", LogLevel.VERBOSE);
        PreBeforeGrabObject?.Invoke(new(playerControllerB, grabbableObject));
        ItemGrabFix.LogDebug("PreBeforeGrabObject?" + (PreBeforeGrabObject != null), LogLevel.VERBOSE);
    }

    internal static void OnBeforeGrabObject(PlayerControllerB playerControllerB, GrabbableObject? grabbableObject) {
        ItemGrabFix.LogDebug("OnBeforeGrabObject!", LogLevel.VERBOSE);
        BeforeGrabObject?.Invoke(new(playerControllerB, grabbableObject));
        ItemGrabFix.LogDebug("BeforeGrabObject?" + (BeforeGrabObject != null), LogLevel.VERBOSE);
    }

    internal static void OnAfterGrabObject(PlayerControllerB playerControllerB, GrabbableObject? grabbableObject) {
        ItemGrabFix.LogDebug("OnAfterGrabObject!", LogLevel.VERBOSE);
        AfterGrabObject?.Invoke(new(playerControllerB, grabbableObject));
        ItemGrabFix.LogDebug("AfterGrabObject? " + (AfterGrabObject != null), LogLevel.VERBOSE);
    }
    // ReSharper disable once EventNeverSubscribedTo.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static event GrabObjectEvent PreBeforeGrabObject;
    public static event GrabObjectEvent BeforeGrabObject;
    public static event GrabObjectEvent AfterGrabObject;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
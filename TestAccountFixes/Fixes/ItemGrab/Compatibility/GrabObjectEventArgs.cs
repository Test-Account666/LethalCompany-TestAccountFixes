using System;
using GameNetcodeStuff;

namespace TestAccountFixes.Fixes.ItemGrab.Compatibility;

public class GrabObjectEventArgs(PlayerControllerB playerControllerB, GrabbableObject? grabbableObject) : EventArgs {
    public readonly PlayerControllerB playerControllerB = playerControllerB;
    public GrabbableObject? grabbableObject = grabbableObject;
}
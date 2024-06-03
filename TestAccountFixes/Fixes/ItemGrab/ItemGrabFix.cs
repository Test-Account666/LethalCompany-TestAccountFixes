using BepInEx.Configuration;
using TestAccountFixes.Core;
using TestAccountFixes.Dependencies;
using TestAccountFixes.Fixes.ItemGrab.Compatibility;

namespace TestAccountFixes.Fixes.ItemGrab;

// ReSharper disable once ClassNeverInstantiated.Global
internal class ItemGrabFix(ConfigFile configFile) : Fix(configFile, "ItemGrab", DESCRIPTION) {
    private const string DESCRIPTION =
        "Breaks the laws of physics to allow grabbing items behind InteractTriggers.";

    // ReSharper disable once MemberCanBePrivate.Global
    internal static ItemGrabFix Instance { get; private set; } = null!;
    internal static ItemGrabFixInputActions itemGrabFixInputActions = null!;

    internal override void Awake() {
        Instance = this;

        if (DependencyChecker.IsBetterItemHandlingInstalled()) {
            TestAccountFixes.Logger.LogInfo("[ItemGrabFix] Found BetterItemHandling enabling support :)");
            BetterItemHandlingSupport.Setup();
        }

        itemGrabFixInputActions = new();

        Patch();
    }

    internal new static void LogDebug(string message, LogLevel logLevel = LogLevel.NORMAL) =>
        ((Fix) Instance).LogDebug(message, logLevel);
}
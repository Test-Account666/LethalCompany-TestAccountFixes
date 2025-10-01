using System.Collections;
using BepInEx.Configuration;
using TestAccountFixes.Core;
using TestAccountFixes.Dependencies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TestAccountFixes.Fixes.AutomaticInventory;

// ReSharper disable once ClassNeverInstantiated.Global
internal class AutomaticInventoryFix(ConfigFile configFile) : Fix(configFile, "AutomaticInventory", DESCRIPTION) {
    private const string DESCRIPTION =
        "Tries to fix inventory issues without user input.";

    // ReSharper disable once MemberCanBePrivate.Global
    internal static AutomaticInventoryFix Instance { get; private set; } = null!;
    private readonly ConfigFile _configFile = configFile;

    internal override void Awake() {
        Instance = this;

        Patch();

        if (DependencyChecker.IsInventoryFixPluginInstalled()) {
            LogDebug("InventoryFixPlugin is installed! Not registering the manual inventory fix key!");
            return;
        }

        var manualInventoryFixInputActions = new ManualInventoryFixInputActions();

        TestAccountFixes.Instance.StartCoroutine(RegisterManualInventoryFixKey(manualInventoryFixInputActions));
    }

    private static IEnumerator RegisterManualInventoryFixKey(
        ManualInventoryFixInputActions manualInventoryFixInputActions) {
        LogDebug("Registering Manual Inventory Fix Key...");
        // No idea why, but Unity really HATES to execute this Coroutine any further after `WaitUntil` returns `true`
        yield return new WaitUntil(() => {
            var registered = manualInventoryFixInputActions.ManualInventoryFixKey is not null;
            LogDebug($"Manual Inventory Fix Key registered? {registered}", LogLevel.VERY_VERBOSE);

            if (!registered) return false;

            LogDebug("Manual Inventory Fix Key registered!");

            manualInventoryFixInputActions.ManualInventoryFixKey!.performed += ReverseThrowingObject;
            return true;
        });
    }

    private static void ReverseThrowingObject(InputAction.CallbackContext context) {
        if (!context.performed) return;

        if (StartOfRound.Instance is null) return;

        var localPlayer = StartOfRound.Instance.localPlayerController;

        if (localPlayer is null) return;

        localPlayer.throwingObject = !localPlayer.throwingObject;

        LogDebug("Manual fix was triggered! ThrowingObject? " + localPlayer.throwingObject);
    }

    internal new static void LogDebug(string message, LogLevel logLevel = LogLevel.NORMAL) => ((Fix) Instance).LogDebug(message, logLevel);
}
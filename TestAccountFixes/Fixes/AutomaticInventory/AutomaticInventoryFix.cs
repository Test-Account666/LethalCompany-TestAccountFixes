using BepInEx.Configuration;
using TestAccountFixes.Core;

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
    }

    internal new static void LogDebug(string message, LogLevel logLevel = LogLevel.NORMAL) =>
        ((Fix) Instance).LogDebug(message, logLevel);
}
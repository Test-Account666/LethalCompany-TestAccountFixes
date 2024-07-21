using BepInEx.Configuration;
using TestAccountFixes.Core;

namespace TestAccountFixes.Fixes.PlayerSpeed;

// ReSharper disable once ClassNeverInstantiated.Global
internal class PlayerSpeedFix(ConfigFile configFile) : Fix(configFile, "PlayerSpeed", DESCRIPTION) {
    private const string DESCRIPTION =
        "The PlayerSpeedFix fixes player's critical injured state while not actually being critically injured.";

    internal static PlayerSpeedFix Instance { get; private set; } = null!;

    internal override void Awake() {
        Instance = this;

        Patch();
    }

    internal new static void LogDebug(string message, LogLevel logLevel = LogLevel.NORMAL) => ((Fix) Instance).LogDebug(message, logLevel);
}
using BepInEx.Configuration;
using TestAccountFixes.Core;

namespace TestAccountFixes.Fixes.SpiderWeb;

// ReSharper disable once ClassNeverInstantiated.Global
internal class SpiderWebFix(ConfigFile configFile) : Fix(configFile, "SpiderWeb", DESCRIPTION) {
    private const string DESCRIPTION =
        "The Spider Web Fix addresses issues related to cobweb interactions, specifically resolving instances where players may become trapped in non-existent cobwebs.";

    // ReSharper disable once MemberCanBePrivate.Global
    internal static SpiderWebFix Instance { get; private set; } = null!;
    internal static ConfigEntry<bool> teleportCobwebs = null!;
    private readonly ConfigFile _configFile = configFile;

    internal override void Awake() {
        Instance = this;

        InitializeConfig();

        Patch();
    }

    private void InitializeConfig() =>
        teleportCobwebs = _configFile.Bind(fixName, "5. Teleport Cobwebs", true,
                                           "If true, will teleport cobwebs somewhere else as soon as they are broken.");

    internal new static void LogDebug(string message, LogLevel logLevel = LogLevel.NORMAL) => ((Fix) Instance).LogDebug(message, logLevel);
}
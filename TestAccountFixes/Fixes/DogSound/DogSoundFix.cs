using BepInEx.Configuration;
using TestAccountFixes.Core;

namespace TestAccountFixes.Fixes.DogSound;

// ReSharper disable once ClassNeverInstantiated.Global
internal class DogSoundFix(ConfigFile configFile) : Fix(configFile, "DogSound", DESCRIPTION) {
    private const string DESCRIPTION =
        "Fixes inconsistency with eyeless dogs, such as their inability to detect shotgun blasts.";

    // ReSharper disable once MemberCanBePrivate.Global
    internal static DogSoundFix Instance { get; private set; } = null!;
    internal static ConfigEntry<bool> fixSilentSprint = null!;
    internal static ConfigEntry<bool> chatIsLoudActually = null!;
    private readonly ConfigFile _configFile = configFile;

    internal override void Awake() {
        Instance = this;

        InitializeConfig();

        Patch();
    }

    private void InitializeConfig() {
        fixSilentSprint = _configFile.Bind(fixName, "5. Fix Silent Sprint", true, "If true, will fix the silent sprint bug");
        chatIsLoudActually = _configFile.Bind(fixName, "6. Chat is loud actually", false,
                                              "If true, chat will be loud. Dogs will be able to hear you sending chat messages");
    }

    internal new static void LogDebug(string message, LogLevel logLevel = LogLevel.NORMAL) => ((Fix) Instance).LogDebug(message, logLevel);
}
using BepInEx.Configuration;
using TestAccountFixes.Core;

namespace TestAccountFixes.Fixes.SpiderDamage;

internal class SpiderDamageFix(ConfigFile configFile) : Fix(configFile, "SpiderDamage", DESCRIPTION, false) {
    private const string DESCRIPTION =
        "The Spooder Damage \"Fix\" will adjust the amount of damage a spider deals.";

    // ReSharper disable once MemberCanBePrivate.Global
    internal static SpiderDamageFix Instance { get; private set; } = null!;
    private readonly ConfigFile _configFile = configFile;
    private ConfigEntry<int> _spooderDamageEntry = null!;

    internal override void Awake() {
        Instance = this;

        InitializeConfig();

        Patch();
    }

    private void InitializeConfig() =>
        _spooderDamageEntry = _configFile.Bind(fixName, "5. Spooder Damage", 20,
                                               "This will change the amount damage that spooders (aka spiders) deal. 90 is the vanilla value.");

    public static int GetSpooderDamage() =>
        Instance?._spooderDamageEntry?.Value ?? 90;

    internal new static void LogDebug(string message, LogLevel logLevel = LogLevel.NORMAL) =>
        ((Fix) Instance).LogDebug(message, logLevel);
}
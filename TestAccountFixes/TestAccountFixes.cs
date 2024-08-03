using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TestAccountCore.Dependencies;
using TestAccountCore.Dependencies.Compatibility;
using TestAccountFixes.Fixes;
using DependencyChecker = TestAccountFixes.Dependencies.DependencyChecker;

namespace TestAccountFixes;

[BepInDependency("Dokge.InventoryFixPlugin", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("Yan01h.BetterItemHandling", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("KoderTech.TelevisionController", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.rune580.LethalCompanyInputUtils")]
[BepInDependency("TestAccount666.TestAccountCore", "1.1.0")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class TestAccountFixes : BaseUnityPlugin {
    public static TestAccountFixes Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    private void Awake() {
        Logger = base.Logger;
        Instance = this;

        if (DependencyChecker.IsLobbyCompatibilityInstalled()) {
            Logger.LogInfo("Found LobbyCompatibility Mod, initializing support :)");
            LobbyCompatibilitySupport.Initialize(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION,
                                                 CompatibilityLevel.ClientOnly, VersionStrictness.Minor);
        }

        InstantiateFixes();

        var printConfigEntries = Config.Bind("General", "Print Config Entries On Startup", false).Value;

        if (printConfigEntries) PrintConfigEntries();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    public void PrintConfigEntries() {
        var configDictionary = new Dictionary<string, string>();

        Config.Do(pair => {
            var definition = pair.Key;
            var value = pair.Value;

            if (value is null) return;

            if (definition.Key.Contains("Description")) return;

            var configEntry = $"{definition.Section} -> {value.BoxedValue} ({value.DefaultValue})";

            var exists = configDictionary.TryGetValue(definition.Key, out var entryString);

            if (!exists) {
                configDictionary.Add(definition.Key, configEntry);
                return;
            }

            entryString += $"\n{configEntry}";

            configDictionary.Remove(definition.Key);
            configDictionary.Add(definition.Key, entryString);
        });

        Logger.LogInfo("Config entries:");
        configDictionary.Do(pair => {
            Logger.LogInfo("~~~~~~~~~~~~~~~~~~~~");
            Logger.LogInfo("Key: " + pair.Key);
            Logger.LogInfo(" ");
            foreach (var value in pair.Value.Split("\n")) Logger.LogInfo(value);
            Logger.LogInfo($"~~~~~~~~~~~~~~~~~~~~");
        });
    }

    private void InstantiateFixes() {
        Logger.LogInfo("Loading fixes...");

        var fixType = typeof(Fix);

        var allFixesAsType = Assembly.GetExecutingAssembly().GetTypes().Where(type => type != null)
                                     .Where(type => fixType.IsAssignableFrom(type))
                                     .Where(type => type != fixType);

        foreach (var type in allFixesAsType) {
            var fix = type.GetConstructors()[0].Invoke([
                Config,
            ]) as Fix;

            fix?.Awake();

            Logger.LogInfo("Loaded fix: " + fix?.fixName);
        }

        Logger.LogInfo("Loaded all fixes!");
    }
}
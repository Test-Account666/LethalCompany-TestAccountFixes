using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TestAccountFixes.Dependencies;
using TestAccountFixes.Fixes;

namespace TestAccountFixes;

[BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("Yan01h.BetterItemHandling", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("KoderTech.TelevisionController", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.rune580.LethalCompanyInputUtils")]
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
            LobbyCompatibilitySupport.Initialize();
        }

        InstantiateFixes();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
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
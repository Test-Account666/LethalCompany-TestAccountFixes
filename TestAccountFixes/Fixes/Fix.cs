using System;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LogLevel = TestAccountFixes.Core.LogLevel;

namespace TestAccountFixes.Fixes;

internal abstract class Fix {
    private readonly string _description;
    private Harmony? Harmony { get; set; }
    internal readonly string fixName;
    private ConfigEntry<bool> _sendDebugMessages = null!;
    private ConfigEntry<bool> _fixEnabled = null!;
    private ConfigEntry<LogLevel> _debugMessageLogLevel = null!;
    private static ManualLogSource Logger { get; set; } = null!;

    internal abstract void Awake();

    protected Fix(ConfigFile configFile, string fixName, string description) {
        this.fixName = fixName;
        _description = description;

        Logger = TestAccountFixes.Logger;

        InitializeConfig(configFile);
    }

    private void InitializeConfig(ConfigFile configFile) {
        _fixEnabled = configFile.Bind(fixName, "1. Enable Fix", true,
                                      "If true, will enable the fix (Requires Restart, if changed via LethalConfig)");

        _sendDebugMessages = configFile.Bind(fixName, "2. Send Debug Messages", false,
                                             "If true, will send additional information. Please turn this on, if you encounter any bugs.");

        _debugMessageLogLevel = configFile.Bind(fixName, "3. Debug Message Log Level", LogLevel.NORMAL,
                                                "The higher the log level, the more spam. Only set this higher if asked.");

        _ = configFile.Bind(fixName, "4. Description", "This entry just exists to describe the fix", _description);
    }

    internal void Patch() {
        if (!_fixEnabled.Value) {
            LogDebug("Skipping this fix, as it is disabled");
            return;
        }

        Harmony ??= new(MyPluginInfo.PLUGIN_GUID + "-" + fixName);

        LogDebug("Patching...");

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
            LogDebug("Got type: " + type.FullName, LogLevel.VERBOSE);
            LogDebug("Passes? " + type?.Namespace?.StartsWith(typeof(Fix).Namespace + "." + fixName), LogLevel.VERBOSE);


            if (!(type?.Namespace?.StartsWith(typeof(Fix).Namespace + "." + fixName) ?? false))
                continue;

            try {
                Harmony.PatchAll(type);
            } catch (TypeLoadException) {
            }
        }

        LogDebug("Finished patching!");
    }

    internal void Unpatch() {
        Harmony ??= new(MyPluginInfo.PLUGIN_GUID + "-" + fixName);

        Logger.LogDebug("Unpatching...");

        Harmony.UnpatchSelf();

        Logger.LogDebug("Finished unpatching!");
    }

    internal void LogDebug(string message, LogLevel logLevel = LogLevel.NORMAL) {
        if (!_sendDebugMessages.Value)
            return;

        if (logLevel > _debugMessageLogLevel.Value)
            return;

        Logger.LogInfo($"[{logLevel}-Debug-{fixName}] {message}");
    }
}
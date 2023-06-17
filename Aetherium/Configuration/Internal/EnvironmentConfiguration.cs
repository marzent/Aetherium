using System;

namespace Aetherium.Configuration.Internal;

/// <summary>
/// Environmental configuration settings.
/// </summary>
internal class EnvironmentConfiguration
{
    /// <summary>
    /// Gets a value indicating whether the Aetherium_NOT_HAVE_PLUGINS setting has been enabled.
    /// </summary>
    public static bool AetheriumNoPlugins { get; } = GetEnvironmentVariable("Aetherium_NOT_HAVE_PLUGINS");

    /// <summary>
    /// Gets a value indicating whether the AetheriumForceDobby setting has been enabled.
    /// </summary>
    public static bool AetheriumForceDobby { get; } = GetEnvironmentVariable("Aetherium_FORCE_DOBBY");

    /// <summary>
    /// Gets a value indicating whether the AetheriumForceMinHook setting has been enabled.
    /// </summary>
    public static bool AetheriumForceFishHook { get; } = GetEnvironmentVariable("Aetherium_FORCE_FISHHOOK");

    /// <summary>
    /// Gets a value indicating whether or not Aetherium context menus should be disabled.
    /// </summary>
    public static bool AetheriumDoContextMenu { get; } = GetEnvironmentVariable("Aetherium_ENABLE_CONTEXTMENU");

    private static bool GetEnvironmentVariable(string name)
        => bool.Parse(Environment.GetEnvironmentVariable(name) ?? "false");
}

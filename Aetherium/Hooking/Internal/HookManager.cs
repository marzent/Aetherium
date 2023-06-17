using System;
using System.Collections.Concurrent;
using Aetherium.Logging.Internal;

namespace Aetherium.Hooking.Internal;

/// <summary>
/// This class manages the final disposition of hooks, cleaning up any that have not reverted their changes.
/// </summary>
[ServiceManager.EarlyLoadedService]
internal class HookManager : IDisposable, IServiceType
{
    /// <summary>
    /// Logger shared with <see cref="Unhooker"/>.
    /// </summary>
    internal static readonly ModuleLog Log = new("HM");

    [ServiceManager.ServiceConstructor]
    private HookManager()
    {
    }

    /// <summary>
    /// Gets sync root object for hook enabling/disabling.
    /// </summary>
    internal static object HookEnableSyncRoot { get; } = new();

    /// <summary>
    /// Gets a static list of tracked and registered hooks.
    /// </summary>
    internal static ConcurrentDictionary<Guid, HookInfo> TrackedHooks { get; } = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        RevertHooks();
        TrackedHooks.Clear();
    }

    private static void RevertHooks()
    {
        foreach (var hookInfo in TrackedHooks.Values)
        {
            hookInfo.Hook.Disable();
        }
    }
}

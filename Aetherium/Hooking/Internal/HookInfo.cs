using System;
using System.Diagnostics;
using System.Reflection;

namespace Aetherium.Hooking.Internal;

/// <summary>
/// Class containing information about registered hooks.
/// </summary>
internal class HookInfo
{
    private ulong? _inProcessMemory = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="HookInfo"/> class.
    /// </summary>
    /// <param name="hook">The tracked hook.</param>
    /// <param name="hookDelegate">The hook delegate.</param>
    /// <param name="assembly">The assembly implementing the hook.</param>
    public HookInfo(IAetheriumHook hook, Delegate hookDelegate, Assembly assembly)
    {
        Hook = hook;
        Delegate = hookDelegate;
        Assembly = assembly;
    }

    /// <summary>
    /// Gets the RVA of the hook.
    /// </summary>
    internal ulong? InProcessMemory
    {
        get
        {
            if (Hook.IsDisposed)
                return 0;

            if (_inProcessMemory == null)
                return null;

            if (_inProcessMemory.Value > 0)
                return _inProcessMemory.Value;

            var p = Process.GetCurrentProcess().MainModule;
            var begin = (ulong)p.BaseAddress.ToInt64();
            var end = begin + (ulong)p.ModuleMemorySize;
            var hookAddr = (ulong)Hook.Address.ToInt64();

            if (hookAddr >= begin && hookAddr <= end)
            {
                return _inProcessMemory = hookAddr - begin;
            }

            return _inProcessMemory = null;
        }
    }

    /// <summary>
    /// Gets the tracked hook.
    /// </summary>
    internal IAetheriumHook Hook { get; }

    /// <summary>
    /// Gets the tracked delegate.
    /// </summary>
    internal Delegate Delegate { get; }

    /// <summary>
    /// Gets the assembly implementing the hook.
    /// </summary>
    internal Assembly Assembly { get; }
}

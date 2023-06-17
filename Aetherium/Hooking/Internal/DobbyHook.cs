using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Aetherium.Hooking.Internal;

/// <summary>
/// Class facilitating hooks via reloaded.
/// </summary>
/// <typeparam name="T">Delegate of the hook.</typeparam>
internal class DobbyHook<T> : Hook<T> where T : Delegate
{
    private readonly T _detour;
    private T _originalFunction;
    private bool _isHookEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="DobbyHook{T}"/> class.
    /// </summary>
    /// <param name="address">A memory address to install a hook.</param>
    /// <param name="detour">Callback function. Delegate must have a same original function prototype.</param>
    /// <param name="callingAssembly">Calling assembly.</param>
    internal DobbyHook(nint address, T detour, Assembly callingAssembly)
        : base(address)
    {
        lock (HookManager.HookEnableSyncRoot)
        {
            _detour = detour;
            _originalFunction = Marshal.GetDelegateForFunctionPointer<T>(address);
            HookManager.TrackedHooks.TryAdd(Guid.NewGuid(), new HookInfo(this, detour, callingAssembly));
        }
    }

    /// <inheritdoc/>
    public override T Original
    {
        get
        {
            CheckDisposed();
            return _originalFunction;
        }
    }

    /// <inheritdoc/>
    public override bool IsEnabled
    {
        get
        {
            CheckDisposed();
            return _isHookEnabled;
        }
    }

    /// <inheritdoc/>
    public override string BackendName => "Dobby";

    /// <inheritdoc/>
    public override void Dispose()
    {
        if (IsDisposed)
            return;

        Disable();

        base.Dispose();
    }

    /// <inheritdoc/>
    public override void Enable()
    {
        CheckDisposed();

        lock (HookManager.HookEnableSyncRoot)
        {
            if (_isHookEnabled) return;
            Dobby.Hook(Address, _detour, out _originalFunction);
            _isHookEnabled = true;
        }
    }

    /// <inheritdoc/>
    public override void Disable()
    {
        CheckDisposed();

        lock (HookManager.HookEnableSyncRoot)
        {
            if (!_isHookEnabled)
                return;

            if (Dobby.DobbyResult.Error == Dobby.Destroy(Address))
                return;

            _isHookEnabled = false;
            _originalFunction = Marshal.GetDelegateForFunctionPointer<T>(Address);
        }
    }
}

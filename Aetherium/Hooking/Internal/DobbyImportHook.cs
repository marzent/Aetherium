using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Aetherium.Hooking.Internal;

/// <summary>
/// Class facilitating hooks via reloaded.
/// </summary>
/// <typeparam name="T">Delegate of the hook.</typeparam>
internal class DobbyImportHook<T> : Hook<T> where T : Delegate
{
    private readonly T _detour;
    private T _originalFunction;
    private bool _isHookEnabled;
    private readonly string _moduleName;
    private readonly string _functionName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DobbyImportHook{T}"/> class.
    /// </summary>
    /// <param name="moduleName">Name of the image, including the extension.</param>
    /// <param name="functionName">Decorated name of the function.</param>
    /// <param name="detour">Callback function. Delegate must have a same original function prototype.</param>
    /// <param name="callingAssembly">Calling assembly.</param>
    internal DobbyImportHook(string moduleName, string functionName, T detour, Assembly callingAssembly)
        : base(Dobby.SymbolResolver(moduleName, functionName))
    {
        lock (HookManager.HookEnableSyncRoot)
        {
            _detour = detour;
            _moduleName = moduleName;
            _functionName = functionName;
            _originalFunction = Marshal.GetDelegateForFunctionPointer<T>(Address);
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
            Dobby.ImportTableReplace(_moduleName, _functionName,_detour, out _originalFunction);
            _isHookEnabled = true;
        }
    }

    /// <inheritdoc/>
    public override void Disable()
    {
        CheckDisposed();

        lock (HookManager.HookEnableSyncRoot)
        {
            // if (!_isHookEnabled)
            //     return;
            //
            // if (Dobby.DobbyResult.Error == Dobby.Destroy(Address))
            //     return;
            //
            // _isHookEnabled = false;
            // _originalFunction = Marshal.GetDelegateForFunctionPointer<T>(Address);
        }
    }
}

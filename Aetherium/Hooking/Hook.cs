using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Aetherium.Hooking.Internal;

namespace Aetherium.Hooking;

/// <summary>
/// Manages a hook which can be used to intercept a call to native function.
/// This class is basically a thin wrapper around the LocalHook type to provide helper functions.
/// </summary>
/// <typeparam name="T">Delegate type to represents a function prototype. This must be the same prototype as original function do.</typeparam>
public class Hook<T> : IDisposable, IAetheriumHook where T : Delegate
{
#pragma warning disable SA1310
    private readonly nint address;

    private readonly Hook<T>? compatHookImpl;

    /// <summary>
    /// Initializes a new instance of the <see cref="Hook{T}"/> class.
    /// </summary>
    /// <param name="address">A memory address to install a hook.</param>
    internal Hook(nint address)
    {
        this.address = address;
    }
    
    /// <summary>
    /// Gets a memory address of the target function.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Hook is already disposed.</exception>
    public nint Address
    {
        get
        {
            CheckDisposed();
            return address;
        }
    }

    /// <summary>
    /// Gets a delegate function that can be used to call the actual function as if function is not hooked yet.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Hook is already disposed.</exception>
    public virtual T Original => compatHookImpl != null ? compatHookImpl!.Original : throw new NotImplementedException();

    /// <summary>
    /// Gets a delegate function that can be used to call the actual function as if function is not hooked yet.
    /// This can be called even after Dispose.
    /// </summary>
    public T OriginalDisposeSafe
    {
        get
        {
            if (compatHookImpl != null)
                return compatHookImpl!.OriginalDisposeSafe;
            if (IsDisposed)
                return Marshal.GetDelegateForFunctionPointer<T>(address);
            return Original;
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not the hook is enabled.
    /// </summary>
    public virtual bool IsEnabled => compatHookImpl != null ? compatHookImpl!.IsEnabled : throw new NotImplementedException();

    /// <summary>
    /// Gets a value indicating whether or not the hook has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public virtual string BackendName => compatHookImpl != null ? compatHookImpl!.BackendName : throw new NotImplementedException();

    /// <summary>
    /// Creates a hook by rewriting import table address.
    /// </summary>
    /// <param name="moduleName">Name of the image, including the extension.</param>
    /// <param name="functionName">Decorated name of the function.</param>
    /// <param name="detour">Callback function. Delegate must have a same original function prototype.</param>
    /// <returns>The hook with the supplied parameters.</returns>
    public static Hook<T> FromImport(string moduleName, string functionName, T detour) 
        => new DobbyImportHook<T>(moduleName, functionName, detour, Assembly.GetCallingAssembly());

    /// <summary>
    /// Creates a hook. Hooking address is inferred by calling to GetProcAddress() function.
    /// The hook is not activated until Enable() method is called.
    /// </summary>
    /// <param name="moduleName">A name of the module currently loaded in the memory. (e.g. ws2_32.dll).</param>
    /// <param name="exportName">A name of the exported function name (e.g. send).</param>
    /// <param name="detour">Callback function. Delegate must have a same original function prototype.</param>
    /// <returns>The hook with the supplied parameters.</returns>
    public static Hook<T> FromSymbol(string moduleName, string exportName, T detour)
        => FromSymbol(moduleName, exportName, detour, false);

    /// <summary>
    /// Creates a hook. Hooking address is inferred by calling to GetProcAddress() function.
    /// The hook is not activated until Enable() method is called.
    /// Please do not use MinHook unless you have thoroughly troubleshot why Reloaded does not work.
    /// </summary>
    /// <param name="moduleName">A name of the module currently loaded in the memory. (e.g. ws2_32.dll).</param>
    /// <param name="exportName">A name of the exported function name (e.g. send).</param>
    /// <param name="detour">Callback function. Delegate must have a same original function prototype.</param>
    /// <param name="useFishHook">Use the FishHook hooking library instead of Dobby.</param>
    /// <returns>The hook with the supplied parameters.</returns>
    public static Hook<T> FromSymbol(string moduleName, string exportName, T detour, bool useFishHook)
    {
        var procAddress = Dobby.SymbolResolver(moduleName, exportName);
        
        if (procAddress == nint.Zero)
            throw new Exception($"Could not get the address of {moduleName}::{exportName}");

        if (useFishHook)
            throw new NotImplementedException();
        
        return new DobbyHook<T>(procAddress, detour, Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// Creates a hook. Hooking address is inferred by calling to GetProcAddress() function.
    /// The hook is not activated until Enable() method is called.
    /// Please do not use MinHook unless you have thoroughly troubleshot why Reloaded does not work.
    /// </summary>
    /// <param name="procAddress">A memory address to install a hook.</param>
    /// <param name="detour">Callback function. Delegate must have a same original function prototype.</param>
    /// <param name="useFishHook">Use the MinHook hooking library instead of Reloaded.</param>
    /// <returns>The hook with the supplied parameters.</returns>
    public static Hook<T> FromAddress(nint procAddress, T detour, bool useFishHook = false)
    {
        if (useFishHook)
            throw new NotImplementedException();
        
        return new DobbyHook<T>(procAddress, detour, Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// Remove a hook from the current process.
    /// </summary>
    public virtual void Dispose()
    {
        if (IsDisposed)
            return;

        compatHookImpl?.Dispose();

        IsDisposed = true;
    }

    /// <summary>
    /// Starts intercepting a call to the function.
    /// </summary>
    public virtual void Enable()
    {
        if (compatHookImpl != null)
            compatHookImpl.Enable();
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// Stops intercepting a call to the function.
    /// </summary>
    public virtual void Disable()
    {
        if (compatHookImpl != null)
            compatHookImpl.Disable();
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// Check if this object has been disposed already.
    /// </summary>
    protected void CheckDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(message: "Hook is already disposed", null);
        }
    }
}

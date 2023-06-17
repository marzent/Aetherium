namespace Aetherium.Hooking;

/// <summary>
/// Interface describing a generic hook.
/// </summary>
public interface IAetheriumHook
{
    /// <summary>
    /// Gets the address to hook.
    /// </summary>
    public nint Address { get; }

    /// <summary>
    /// Gets a value indicating whether or not the hook is enabled.
    /// </summary>
    public bool IsEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether or not the hook is disposed.
    /// </summary>
    public bool IsDisposed { get; }

    /// <summary>
    /// Gets the name of the hooking backend used for the hook.
    /// </summary>
    public string BackendName { get; }

    /// <summary>
    /// Stops intercepting a call to the function.
    /// </summary>
    public void Disable();
}

using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.ObjectiveC;

public struct NSWindow
{
    public readonly nint NativePtr;
    public static implicit operator nint(NSWindow c) => c.NativePtr;
    public NSWindow(nint ptr) => NativePtr = ptr;

    public NSView contentView => objc_msgSend<NSView>(NativePtr, "contentView");
    
    public NSScreen screen => objc_msgSend<NSScreen>(NativePtr, "screen");
}
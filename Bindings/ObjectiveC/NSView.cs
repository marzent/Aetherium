using System.Runtime.InteropServices;
using Bindings.Metal;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.ObjectiveC;

public struct NSView
{
    public readonly nint NativePtr;
    
    public static implicit operator nint(NSView nsView) => nsView.NativePtr;

    public NSView(nint ptr) => NativePtr = ptr;

    public Bool8 wantsLayer
    {
        get => bool8_objc_msgSend(NativePtr, "wantsLayer");
        set => objc_msgSend(NativePtr, "setWantsLayer:", value);
    }

    public nint layer
    {
        get => IntPtr_objc_msgSend(NativePtr, "layer");
        set => objc_msgSend(NativePtr, "setLayer:", value);
    }

    public CGRect frame =>
        RuntimeInformation.ProcessArchitecture == Architecture.Arm64
            ? CGRect_objc_msgSend(NativePtr, "frame")
            : objc_msgSend_stret<CGRect>(NativePtr, "frame");

    public CGRect bounds =>
        RuntimeInformation.ProcessArchitecture == Architecture.Arm64
            ? CGRect_objc_msgSend(NativePtr, "bounds")
            : objc_msgSend_stret<CGRect>(NativePtr, "bounds");

    public NSWindow? window
    {
        get
        {
            var windowPtr = IntPtr_objc_msgSend(NativePtr, "window");
            return windowPtr == nint.Zero ? null : new NSWindow(windowPtr);
        }
    }
}
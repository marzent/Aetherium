using System.Runtime.InteropServices;
using Aetherium.Bindings.ObjectiveC;

namespace Aetherium.Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MTLBuffer
{
    public readonly nint NativePtr;
    public MTLBuffer(nint ptr) => NativePtr = ptr;
    public bool IsNull => NativePtr == nint.Zero;

    public void* contents() => ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, sel_contents).ToPointer();

    public nuint length => ObjectiveCRuntime.UIntPtr_objc_msgSend(NativePtr, sel_length);

    public void didModifyRange(NSRange range)
        => ObjectiveCRuntime.objc_msgSend(NativePtr, sel_didModifyRange, range);

    public void addDebugMarker(NSString marker, NSRange range)
        => ObjectiveCRuntime.objc_msgSend(NativePtr, sel_addDebugMarker, marker.NativePtr, range);

    public void removeAllDebugMarkers()
        => ObjectiveCRuntime.objc_msgSend(NativePtr, sel_removeAllDebugMarkers);

    private static readonly Selector sel_contents = "contents";
    private static readonly Selector sel_length = "length";
    private static readonly Selector sel_didModifyRange = "didModifyRange:";
    private static readonly Selector sel_addDebugMarker = "addDebugMarker:range:";
    private static readonly Selector sel_removeAllDebugMarkers = "removeAllDebugMarkers";
}
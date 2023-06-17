using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.ObjectiveC;

public struct NSArray
{
    public readonly nint NativePtr;
    public NSArray(nint ptr) => NativePtr = ptr;

    public nuint count => UIntPtr_objc_msgSend(NativePtr, sel_count);
    private static readonly Selector sel_count = "count";
}
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.ObjectiveC;

public struct NSArray
{
    public readonly nint NativePtr;
    
    public NSArray(nint ptr) => NativePtr = ptr;

    public nint firstObject => IntPtr_objc_msgSend(NativePtr, "firstObject");

    public nuint count => UIntPtr_objc_msgSend(NativePtr, "count");
}
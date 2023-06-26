using Bindings.ObjectiveC;

namespace Bindings.Metal;

public struct MTLFunctionConstantValues
{
    public readonly nint NativePtr;

    public static MTLFunctionConstantValues New()
    {
        return s_class.AllocInit<MTLFunctionConstantValues>();
    }

    public void Release() => ObjectiveCRuntime.release(NativePtr);

    public unsafe void setConstantValuetypeatIndex(void* value, MTLDataType type, nuint index)
    {
        ObjectiveCRuntime.objc_msgSend(NativePtr, sel_setConstantValuetypeatIndex, value, (uint)type, index);
    }

    private static readonly ObjCClass s_class = new ObjCClass(nameof(MTLFunctionConstantValues));
    private static readonly Selector sel_setConstantValuetypeatIndex = "setConstantValue:type:atIndex:";
}
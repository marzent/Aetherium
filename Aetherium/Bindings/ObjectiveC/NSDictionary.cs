namespace Aetherium.Bindings.ObjectiveC;

public struct NSDictionary
{
    public readonly nint NativePtr;

    public nuint count => ObjectiveCRuntime.UIntPtr_objc_msgSend(NativePtr, "count");
}
namespace Aetherium.Bindings.ObjectiveC;

public struct ObjectiveCMethod
{
    public readonly nint NativePtr;
    public ObjectiveCMethod(nint ptr) => NativePtr = ptr;
    public static implicit operator nint(ObjectiveCMethod method) => method.NativePtr;
    public static implicit operator ObjectiveCMethod(nint ptr) => new ObjectiveCMethod(ptr);

    public Selector GetSelector() => ObjectiveCRuntime.method_getName(this);
    public string GetName() => GetSelector().Name;
}
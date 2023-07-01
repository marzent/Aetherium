using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.Metal;

public struct MTLSamplerState
{
    public readonly nint NativePtr;
    public void Release() => release(NativePtr);
}
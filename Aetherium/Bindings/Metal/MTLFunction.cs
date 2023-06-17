using Aetherium.Bindings.ObjectiveC;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

public struct MTLFunction
{
    public readonly nint NativePtr;
    public MTLFunction(nint ptr) => NativePtr = ptr;

    public void Release() => release(NativePtr);

    public NSDictionary functionConstantsDictionary => objc_msgSend<NSDictionary>(NativePtr, sel_functionConstantsDictionary);

    private static readonly Selector sel_functionConstantsDictionary = "functionConstantsDictionary";
}
using System.Runtime.InteropServices;
using Bindings.ObjectiveC;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLLibrary
{
    public readonly nint NativePtr;
    public MTLLibrary(nint ptr) => NativePtr = ptr;

    public MTLFunction newFunctionWithName(string name)
    {
        NSString nameNSS = NSString.New(name);
        nint function = IntPtr_objc_msgSend(NativePtr, sel_newFunctionWithName, nameNSS);
        release(nameNSS.NativePtr);
        return new MTLFunction(function);
    }

    public MTLFunction newFunctionWithNameConstantValues(string name, MTLFunctionConstantValues constantValues)
    {
        var nameNSS = NSString.New(name);
        var function = IntPtr_objc_msgSend(
            NativePtr,
            sel_newFunctionWithNameConstantValues,
            nameNSS.NativePtr,
            constantValues.NativePtr,
            out NSError error);
        release(nameNSS.NativePtr);

        if (function == nint.Zero)
        {
            throw new Exception($"Failed to create MTLFunction: {error.localizedDescription}");
        }

        return new MTLFunction(function);
    }

    private static readonly Selector sel_newFunctionWithName = "newFunctionWithName:";
    private static readonly Selector sel_newFunctionWithNameConstantValues = "newFunctionWithName:constantValues:error:";
}
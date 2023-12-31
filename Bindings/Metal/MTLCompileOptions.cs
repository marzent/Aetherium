using System.Runtime.InteropServices;
using Bindings.ObjectiveC;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLCompileOptions
{
    public readonly nint NativePtr;

    public static implicit operator nint(MTLCompileOptions mco) => mco.NativePtr;

    public static MTLCompileOptions New()
    {
        return s_class.AllocInit<MTLCompileOptions>();
    }

    public void Release() => ObjectiveCRuntime.release(NativePtr);

    public Bool8 fastMathEnabled
    {
        get => bool8_objc_msgSend(NativePtr, sel_fastMathEnabled);
        set => objc_msgSend(NativePtr, sel_setFastMathEnabled, value);
    }

    public MTLLanguageVersion languageVersion
    {
        get => (MTLLanguageVersion)uint_objc_msgSend(NativePtr, sel_languageVersion);
        set => objc_msgSend(NativePtr, sel_setLanguageVersion, (uint)value);
    }

    private static readonly ObjCClass s_class = new ObjCClass(nameof(MTLCompileOptions));
    private static readonly Selector sel_fastMathEnabled = "fastMathEnabled";
    private static readonly Selector sel_setFastMathEnabled = "setFastMathEnabled:";
    private static readonly Selector sel_languageVersion = "languageVersion";
    private static readonly Selector sel_setLanguageVersion = "setLanguageVersion:";
}
using Bindings.Metal;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.ObjectiveC;

public unsafe struct NSString
{
    public readonly nint NativePtr;
    public NSString(nint ptr) => NativePtr = ptr;
    public static implicit operator nint(NSString nss) => nss.NativePtr;
    public void Release() => release(NativePtr);

    public static NSString New(string s)
    {
        var nss = s_class.Alloc<NSString>();

        fixed (char* utf16Ptr = s)
        {
            nuint length = (nuint)s.Length;
            nint newString = IntPtr_objc_msgSend(nss, sel_initWithCharacters, (nint)utf16Ptr, length);
            return new NSString(newString);
        }
    }

    public string? GetValue()
    {
        if (NativePtr == nint.Zero) return null;
        byte* utf8Ptr = bytePtr_objc_msgSend(NativePtr, sel_utf8String);
        return utf8Ptr == (byte*)0 ? null : MTLUtil.GetUtf8String(utf8Ptr);
    }

    private static readonly ObjCClass s_class = new ObjCClass(nameof(NSString));
    private static readonly Selector sel_initWithCharacters = "initWithCharacters:length:";
    private static readonly Selector sel_utf8String = "UTF8String";
}
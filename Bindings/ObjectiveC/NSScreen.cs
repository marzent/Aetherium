using System.Runtime.InteropServices;
using Bindings.Metal;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.ObjectiveC;

public struct NSScreen
{
    public readonly nint NativePtr;
    public static implicit operator nint(NSScreen nsScreen) => nsScreen.NativePtr;

    public NSScreen(nint ptr) => NativePtr = ptr;

    public CGRect frame =>
        RuntimeInformation.ProcessArchitecture == Architecture.Arm64
            ? CGRect_objc_msgSend(NativePtr, "frame")
            : objc_msgSend_stret<CGRect>(NativePtr, "frame");

    public CGFloat backingScaleFactor => CGFloat_objc_msgSend(NativePtr, "backingScaleFactor");
    
    public static NSScreen mainScreen => new(s_class.GetProperty("mainScreen"));

    private static readonly ObjCClass s_class = new(nameof(NSScreen));
}
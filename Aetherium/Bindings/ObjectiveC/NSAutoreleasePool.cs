using System;

namespace Aetherium.Bindings.ObjectiveC;

public struct NSAutoreleasePool : IDisposable
{
    private static readonly ObjCClass s_class = new ObjCClass(nameof(NSAutoreleasePool));
    public readonly nint NativePtr;
    public NSAutoreleasePool(nint ptr) => NativePtr = ptr;

    public static NSAutoreleasePool Begin()
    {
        return s_class.AllocInit<NSAutoreleasePool>();
    }

    public void Dispose()
    {
        ObjectiveCRuntime.release(this.NativePtr);
    }
}
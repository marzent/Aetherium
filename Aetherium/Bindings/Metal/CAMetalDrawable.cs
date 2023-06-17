using System.Runtime.InteropServices;
using Aetherium.Bindings.ObjectiveC;
using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct CAMetalDrawable
{
    public readonly nint NativePtr;
    public CAMetalDrawable(nint ptr) => NativePtr = ptr;
    public bool IsNull => NativePtr == nint.Zero;
    public MTLTexture texture => objc_msgSend<MTLTexture>(NativePtr, Selectors.texture);
}
using System.Runtime.InteropServices;
using Bindings.ObjectiveC;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MTLTexture
{
    public readonly nint NativePtr;

    public MTLTexture(nint ptr) => NativePtr = ptr;
    public bool IsNull => NativePtr == nint.Zero;
    public void Release() => release(NativePtr);

    public void replaceRegion(
        MTLRegion region,
        nuint mipmapLevel,
        nuint slice,
        void* pixelBytes,
        nuint bytesPerRow,
        nuint bytesPerImage)
    {
        objc_msgSend(NativePtr, sel_replaceRegion,
            region,
            mipmapLevel,
            slice,
            (nint)pixelBytes,
            bytesPerRow,
            bytesPerImage);
    }

    public MTLTexture newTextureView(
        MTLPixelFormat pixelFormat,
        MTLTextureType textureType,
        NSRange levelRange,
        NSRange sliceRange)
    {
        nint ret = IntPtr_objc_msgSend(NativePtr, sel_newTextureView,
            (uint)pixelFormat, (uint)textureType, levelRange, sliceRange);
        return new MTLTexture(ret);
    }
    
    public uint Width => uint_objc_msgSend(NativePtr, sel_width);

    public uint Height => uint_objc_msgSend(NativePtr, sel_height);
    
    public MTLPixelFormat pixelFormat => (MTLPixelFormat)uint_objc_msgSend(NativePtr, sel_pixelFormat);

    private static readonly Selector sel_replaceRegion = "replaceRegion:mipmapLevel:slice:withBytes:bytesPerRow:bytesPerImage:";
    private static readonly Selector sel_newTextureView = "newTextureViewWithPixelFormat:textureType:levels:slices:";
    private static readonly Selector sel_width = "width";
    private static readonly Selector sel_height = "height";
    private static readonly Selector sel_pixelFormat = "pixelFormat";
}
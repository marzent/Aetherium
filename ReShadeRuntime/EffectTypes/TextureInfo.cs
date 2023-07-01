using System.Runtime.InteropServices;
using Bindings.Metal;

namespace ReShadeRuntime.EffectTypes;

internal class TextureInfo
{
    public enum TextureFormat
    {
        TextureFormatUnknown,
        R8,
        R16,
        R16F,
        R32F,
        RG8,
        RG16,
        RG16F,
        RG32F,
        RGBA8,
        RGBA16,
        RGBA16F,
        RGBA32F,
        RGB10A2
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct CTextureInfo
    {
        public readonly uint id;
        public readonly uint binding;
        [MarshalAs(UnmanagedType.LPStr)] 
        public readonly string name;
        [MarshalAs(UnmanagedType.LPStr)] 
        public readonly string semantic;
        [MarshalAs(UnmanagedType.LPStr)] 
        public readonly string unique_name;
        public readonly nint annotations;
        public readonly ulong annotations_size;
        public readonly uint width;
        public readonly uint height;
        public readonly ushort levels;
        public readonly TextureFormat format;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool render_target;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool storage_access;
    }
    
    public int Id { get; }
    public int Binding { get; }
    public string Name { get; }
    public string Semantic { get; }
    public string UniqueName { get; }
    public Annotation[] Annotations { get; }
    public int Width { get; }
    public int Height { get; }
    public ushort Levels { get; }
    public TextureFormat Format { get; }
    public bool RenderTarget { get; }
    public bool StorageAccess { get; }

    public TextureInfo(nint cStruct)
    {
        var cTextureInfo = Marshal.PtrToStructure<CTextureInfo>(cStruct);
        Id = (int)cTextureInfo.id;
        Binding = (int)cTextureInfo.binding;
        Name = cTextureInfo.name;
        Semantic = cTextureInfo.semantic;
        UniqueName = cTextureInfo.unique_name;
        Annotations = Enumerable.Range(0, (int)cTextureInfo.annotations_size)
            .Select(i => new Annotation(Marshal.ReadIntPtr(cTextureInfo.annotations + 8 * i)))
            .ToArray();
        Width = (int)cTextureInfo.width;
        Height = (int)cTextureInfo.height;
        Levels = cTextureInfo.levels;
        Format = cTextureInfo.format;
        RenderTarget = cTextureInfo.render_target;
        StorageAccess = cTextureInfo.storage_access;
    }
    
    public MTLTextureDescriptor ToDescriptor()
    {
        var textureDescriptor = MTLTextureDescriptor.New();
        textureDescriptor.width = (ulong)Width;
        textureDescriptor.height = (ulong)Height;
        textureDescriptor.depth = Levels;

        textureDescriptor.pixelFormat = Format switch
        {
            TextureFormat.R8 => MTLPixelFormat.R8Unorm,
            TextureFormat.R16 => MTLPixelFormat.R16Unorm,
            TextureFormat.R16F => MTLPixelFormat.R16Float,
            TextureFormat.R32F => MTLPixelFormat.R32Float,
            TextureFormat.RG8 => MTLPixelFormat.RG8Unorm,
            TextureFormat.RG16 => MTLPixelFormat.RG16Unorm,
            TextureFormat.RG16F => MTLPixelFormat.RG16Float,
            TextureFormat.RG32F => MTLPixelFormat.RG32Float,
            TextureFormat.RGBA8 => MTLPixelFormat.RGBA8Unorm,
            TextureFormat.RGBA16 => MTLPixelFormat.RGBA16Unorm,
            TextureFormat.RGBA16F => MTLPixelFormat.RGBA16Float,
            TextureFormat.RGBA32F => MTLPixelFormat.RGBA32Float,
            TextureFormat.RGB10A2 => MTLPixelFormat.RGB10A2Unorm,
            _ => MTLPixelFormat.RGBA8Unorm
        };

        textureDescriptor.textureUsage = MTLTextureUsage.ShaderRead;
        if (RenderTarget) textureDescriptor.textureUsage |= MTLTextureUsage.RenderTarget;

        textureDescriptor.storageMode = StorageAccess ? MTLStorageMode.Managed : MTLStorageMode.Private;

        return textureDescriptor;
    }
}
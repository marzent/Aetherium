using System.Runtime.InteropServices;
using Bindings.Metal;

namespace ReShadeRuntime.EffectTypes;

internal class SamplerInfo
{
    public enum FilterMode
    {
        MinMagMipPoint = 0x0,
        MinMagPointMipLinear = 0x1,
        MinPointMagLinearMipPoint = 0x4,
        MinPointMagMipLinear = 0x5,
        MinLinearMagMipPoint = 0x10,
        MinLinearMagPointMipLinear = 0x11,
        MinMagLinearMipPoint = 0x14,
        MinMagMipLinear = 0x15
    }

    private static MTLSamplerMipFilter TranslateSamplerMipFilter(FilterMode filterMode)
    {
        switch (filterMode)
        {
            case FilterMode.MinMagMipPoint:
            case FilterMode.MinPointMagLinearMipPoint:
            case FilterMode.MinLinearMagMipPoint:
            case FilterMode.MinMagLinearMipPoint:
                return MTLSamplerMipFilter.Nearest;
            case FilterMode.MinMagPointMipLinear:
            case FilterMode.MinPointMagMipLinear:
            case FilterMode.MinLinearMagPointMipLinear:
            case FilterMode.MinMagMipLinear:
                return MTLSamplerMipFilter.Linear;
            default:
                return MTLSamplerMipFilter.NotMipmapped;
        }
    }
    
    private static MTLSamplerMinMagFilter TranslateSamplerMinFilter(FilterMode filterMode)
    {
        switch (filterMode)
        {
            case FilterMode.MinMagMipPoint:
            case FilterMode.MinPointMagLinearMipPoint:
            case FilterMode.MinMagPointMipLinear:
            case FilterMode.MinPointMagMipLinear:
                return MTLSamplerMinMagFilter.Nearest;
            case FilterMode.MinLinearMagPointMipLinear:
            case FilterMode.MinLinearMagMipPoint:
            case FilterMode.MinMagLinearMipPoint:
            case FilterMode.MinMagMipLinear:
                return MTLSamplerMinMagFilter.Linear;
            default:
                return MTLSamplerMinMagFilter.Nearest;
        }
    }
    
    private static MTLSamplerMinMagFilter TranslateSamplerMagFilter(FilterMode filterMode)
    {
        switch (filterMode)
        {
            case FilterMode.MinMagMipPoint:
            case FilterMode.MinLinearMagMipPoint:
            case FilterMode.MinMagPointMipLinear:
            case FilterMode.MinLinearMagPointMipLinear:
                return MTLSamplerMinMagFilter.Nearest;
            case FilterMode.MinPointMagMipLinear:
            case FilterMode.MinPointMagLinearMipPoint:
            case FilterMode.MinMagLinearMipPoint:
            case FilterMode.MinMagMipLinear:
                return MTLSamplerMinMagFilter.Linear;
            default:
                return MTLSamplerMinMagFilter.Nearest;
        }
    }
    
    public enum TextureAddressMode
    {
        Wrap = 1,
        Mirror,
        Clamp,
        Border
    }
    
    private static MTLSamplerAddressMode TranslateAddressMode(TextureAddressMode address) =>
        address switch
        {
            TextureAddressMode.Wrap => MTLSamplerAddressMode.Repeat,
            TextureAddressMode.Mirror => MTLSamplerAddressMode.MirrorRepeat,
            TextureAddressMode.Clamp => MTLSamplerAddressMode.ClampToEdge,
            TextureAddressMode.Border => MTLSamplerAddressMode.ClampToZero,
            _ => MTLSamplerAddressMode.ClampToEdge
        };

    [StructLayout(LayoutKind.Sequential)]
    private struct CSamplerInfo
    {
        public readonly uint id;
        public readonly uint binding;
        public readonly uint texture_binding;
        [MarshalAs(UnmanagedType.LPStr)] 
        public readonly string name;
        [MarshalAs(UnmanagedType.LPStr)] 
        public readonly string unique_name;
        [MarshalAs(UnmanagedType.LPStr)] 
        public readonly string texture_name;
        public readonly nint annotations;
        public readonly ulong annotations_size;
        public readonly FilterMode filter;
        public readonly TextureAddressMode address_u;
        public readonly TextureAddressMode address_v;
        public readonly TextureAddressMode address_w;
        public readonly float min_lod;
        public readonly float max_lod;
        public readonly float lod_bias;
        public readonly byte srgb;
    }
    
    public int Id { get; }
    public int Binding { get; }
    public int TextureBinding { get; }
    public string Name { get; }
    public string UniqueName { get; }
    public string TextureName { get; }
    public Annotation[] Annotations { get; }
    public FilterMode Filter { get; }
    public TextureAddressMode AddressU { get; }
    public TextureAddressMode AddressV { get; }
    public TextureAddressMode AddressW { get; }
    public float MinLod { get; }
    public float MaxLod { get; }
    public float LodBias { get; }
    public byte Srgb { get; }

    public SamplerInfo(nint cStruct)
    {
        var cSamplerInfo = Marshal.PtrToStructure<CSamplerInfo>(cStruct);
        Id = (int)cSamplerInfo.id;
        Binding = (int)cSamplerInfo.binding;
        TextureBinding = (int)cSamplerInfo.texture_binding;
        Name = cSamplerInfo.name;
        UniqueName = cSamplerInfo.unique_name;
        TextureName = cSamplerInfo.texture_name;
        Annotations = Enumerable.Range(0, (int)cSamplerInfo.annotations_size)
            .Select(i => new Annotation(cSamplerInfo.annotations + 8 * i))
            .ToArray();
        Filter = cSamplerInfo.filter;
        AddressU = cSamplerInfo.address_u;
        AddressV = cSamplerInfo.address_v;
        AddressW = cSamplerInfo.address_w;
        MinLod = cSamplerInfo.min_lod;
        MaxLod = cSamplerInfo.max_lod;
        LodBias = cSamplerInfo.lod_bias;
        Srgb = cSamplerInfo.srgb;
    }
    
    public MTLSamplerDescriptor ToDescriptor()
    {
        var samplerDescriptor = MTLSamplerDescriptor.New();
        samplerDescriptor.sAddressMode = TranslateAddressMode(AddressU);
        samplerDescriptor.tAddressMode = TranslateAddressMode(AddressV);
        samplerDescriptor.rAddressMode = TranslateAddressMode(AddressW);
        samplerDescriptor.magFilter = TranslateSamplerMagFilter(Filter);
        samplerDescriptor.minFilter = TranslateSamplerMinFilter(Filter);
        samplerDescriptor.mipFilter = TranslateSamplerMipFilter(Filter);
        samplerDescriptor.lodMaxClamp = MaxLod;
        samplerDescriptor.lodMinClamp = MinLod;
        // seems there is no LOD bias in metal directly outside of adjusting shaders themselves
        // not sure about the srgb stuff either
        return samplerDescriptor;
    }
}
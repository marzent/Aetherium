using System.Runtime.InteropServices;
using Bindings.Metal;

namespace ReShadeRuntime.EffectTypes;

internal class PassInfo
{
    public enum PassBlendOp
    {
        Add = 1,
        Subtract,
        ReverseSubtract,
        Min,
        Max
    }
    
    public static MTLBlendOperation ConvertToMTLBlendOperation(PassBlendOp blendOp) =>
        blendOp switch
        {
            PassBlendOp.Add => MTLBlendOperation.Add,
            PassBlendOp.Subtract => MTLBlendOperation.Subtract,
            PassBlendOp.ReverseSubtract => MTLBlendOperation.ReverseSubtract,
            PassBlendOp.Min => MTLBlendOperation.Min,
            PassBlendOp.Max => MTLBlendOperation.Max,
            _ => throw new ArgumentException("Invalid blend operation")
        };

    public enum PassBlendFunc
    {
        Zero,
        One,
        SrcColor,
        SrcAlpha,
        InvSrcColor,
        InvSrcAlpha,
        DstColor,
        DstAlpha,
        InvDstColor,
        InvDstAlpha
    }
    
    public static MTLBlendFactor ConvertToMTLBlendFactor(PassBlendFunc blendFunc) =>
        blendFunc switch
        {
            PassBlendFunc.Zero => MTLBlendFactor.Zero,
            PassBlendFunc.One => MTLBlendFactor.One,
            PassBlendFunc.SrcColor => MTLBlendFactor.SourceColor,
            PassBlendFunc.SrcAlpha => MTLBlendFactor.SourceAlpha,
            PassBlendFunc.InvSrcColor => MTLBlendFactor.OneMinusSourceColor,
            PassBlendFunc.InvSrcAlpha => MTLBlendFactor.OneMinusSourceAlpha,
            PassBlendFunc.DstColor => MTLBlendFactor.DestinationColor,
            PassBlendFunc.DstAlpha => MTLBlendFactor.DestinationAlpha,
            PassBlendFunc.InvDstColor => MTLBlendFactor.OneMinusDestinationColor,
            PassBlendFunc.InvDstAlpha => MTLBlendFactor.OneMinusDestinationAlpha,
            _ => throw new ArgumentException("Invalid blend function")
        };

    public enum PassStencilOp
    {
        Zero,
        Keep,
        Invert,
        Replace,
        Incr,
        IncrSat,
        Decr,
        DecrSat
    }
    
    public static MTLStencilOperation ConvertToMTLStencilOperation(PassStencilOp stencilOp)
    {
        return stencilOp switch
        {
            PassStencilOp.Zero => MTLStencilOperation.Zero,
            PassStencilOp.Keep => MTLStencilOperation.Keep,
            PassStencilOp.Invert => MTLStencilOperation.Invert,
            PassStencilOp.Replace => MTLStencilOperation.Replace,
            PassStencilOp.Incr => MTLStencilOperation.IncrementWrap,
            PassStencilOp.IncrSat => MTLStencilOperation.IncrementClamp,
            PassStencilOp.Decr => MTLStencilOperation.DecrementWrap,
            PassStencilOp.DecrSat => MTLStencilOperation.DecrementClamp,
            _ => throw new ArgumentException("Invalid stencil operation")
        };
    }


    public enum PassStencilFunc
    {
        Never,
        Equal,
        NotEqual,
        Less,
        LessEqual,
        Greater,
        GreaterEqual,
        Always
    }
    
    public static MTLCompareFunction ConvertToMTLCompareFunction(PassStencilFunc stencilFunc)
    {
        return stencilFunc switch
        {
            PassStencilFunc.Never => MTLCompareFunction.Never,
            PassStencilFunc.Less => MTLCompareFunction.Less,
            PassStencilFunc.Equal => MTLCompareFunction.Equal,
            PassStencilFunc.NotEqual => MTLCompareFunction.NotEqual,
            PassStencilFunc.LessEqual => MTLCompareFunction.LessEqual,
            PassStencilFunc.Greater => MTLCompareFunction.Greater,
            PassStencilFunc.GreaterEqual => MTLCompareFunction.GreaterEqual,
            PassStencilFunc.Always => MTLCompareFunction.Always,
            _ => throw new ArgumentException("Invalid stencil function")
        };
    }

    public enum PrimitiveTopology
    {
        PointList = 1,
        LineList,
        LineStrip,
        TriangleList,
        TriangleStrip
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CPassInfo
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string name;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.LPStr, SizeConst = 8)]
        public readonly string[] render_target_names;
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string vs_entry_point;
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string ps_entry_point;
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string cs_entry_point;
        public readonly byte generate_mipmaps;
        public readonly byte clear_render_targets;
        public readonly byte srgb_write_enable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly byte[] blend_enable;
        public readonly byte stencil_enable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly byte[] color_write_mask;
        public readonly byte stencil_read_mask;
        public readonly byte stencil_write_mask;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly PassBlendOp[] blend_op;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly PassBlendOp[] blend_op_alpha;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly PassBlendFunc[] src_blend;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly PassBlendFunc[] dest_blend;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly PassBlendFunc[] src_blend_alpha;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly PassBlendFunc[] dest_blend_alpha;
        public readonly PassStencilFunc stencil_comparison_func;
        public readonly uint stencil_reference_value;
        public readonly PassStencilOp stencil_op_pass;
        public readonly PassStencilOp stencil_op_fail;
        public readonly PassStencilOp stencil_op_depth_fail;
        public readonly uint num_vertices;
        public readonly PrimitiveTopology topology;
        public readonly uint viewport_width;
        public readonly uint viewport_height;
        public readonly uint viewport_dispatch_z;
        public readonly nint samplers;
        public readonly ulong samplers_size;
        public readonly nint storages;
        public readonly ulong storages_size;
    }
    
    public string Name { get; }
    public string[] RenderTargetNames { get; }
    public string VertexShaderEntryPoint { get; }
    public string FragmentShaderEntryPoint { get; }
    public string ComputeShaderEntryPoint { get; }
    public byte GenerateMipmaps { get; }
    public byte ClearRenderTargets { get; }
    public byte SrgbWriteEnable { get; }
    public byte[] BlendEnable { get; }
    public byte StencilEnable { get; }
    public byte[] ColorWriteMask { get; }
    public byte StencilReadMask { get; }
    public byte StencilWriteMask { get; }
    public PassBlendOp[] BlendOp { get; }
    public PassBlendOp[] BlendOpAlpha { get; }
    public PassBlendFunc[] SrcBlend { get; }
    public PassBlendFunc[] DestBlend { get; }
    public PassBlendFunc[] SrcBlendAlpha { get; }
    public PassBlendFunc[] DestBlendAlpha { get; }
    public PassStencilFunc StencilComparisonFunc { get; }
    public uint StencilReferenceValue { get; }
    public PassStencilOp StencilOpPass { get; }
    public PassStencilOp StencilOpFail { get; }
    public PassStencilOp StencilOpDepthFail { get; }
    public uint NumVertices { get; }
    public PrimitiveTopology Topology { get; }
    public int ViewportWidth { get; }
    public int ViewportHeight { get; }
    public int ViewportDispatchZ { get; }
    public SamplerInfo[] Samplers { get; }
    public StorageInfo[] Storages { get; }

    public MTLPrimitiveType PrimitiveType =>
        Topology switch
        {
            PrimitiveTopology.PointList => MTLPrimitiveType.Point,
            PrimitiveTopology.LineList => MTLPrimitiveType.Line,
            PrimitiveTopology.LineStrip => MTLPrimitiveType.LineStrip,
            PrimitiveTopology.TriangleList => MTLPrimitiveType.Triangle,
            PrimitiveTopology.TriangleStrip => MTLPrimitiveType.TriangleStrip,
            _ => throw new ArgumentOutOfRangeException()
        };

    public PassInfo(nint cStruct)
    {
        var cEntryPoint = Marshal.PtrToStructure<CPassInfo>(cStruct);
        Name = cEntryPoint.name;
        RenderTargetNames = cEntryPoint.render_target_names;
        VertexShaderEntryPoint = cEntryPoint.vs_entry_point;
        FragmentShaderEntryPoint = cEntryPoint.ps_entry_point;
        ComputeShaderEntryPoint = cEntryPoint.cs_entry_point;
        GenerateMipmaps = cEntryPoint.generate_mipmaps;
        ClearRenderTargets = cEntryPoint.clear_render_targets;
        SrgbWriteEnable = cEntryPoint.srgb_write_enable;
        BlendEnable = cEntryPoint.blend_enable;
        StencilEnable = cEntryPoint.stencil_enable;
        ColorWriteMask = cEntryPoint.color_write_mask;
        StencilReadMask = cEntryPoint.stencil_read_mask;
        StencilWriteMask = cEntryPoint.stencil_write_mask;
        BlendOp = cEntryPoint.blend_op;
        BlendOpAlpha = cEntryPoint.blend_op_alpha;
        SrcBlend = cEntryPoint.src_blend;
        DestBlend = cEntryPoint.dest_blend;
        SrcBlendAlpha = cEntryPoint.src_blend_alpha;
        DestBlendAlpha = cEntryPoint.dest_blend_alpha;
        StencilComparisonFunc = cEntryPoint.stencil_comparison_func;
        StencilReferenceValue = cEntryPoint.stencil_reference_value;
        StencilOpPass = cEntryPoint.stencil_op_pass;
        StencilOpFail = cEntryPoint.stencil_op_fail;
        StencilOpDepthFail = cEntryPoint.stencil_op_depth_fail;
        NumVertices = cEntryPoint.num_vertices;
        Topology = cEntryPoint.topology;
        ViewportWidth = (int)cEntryPoint.viewport_width;
        ViewportHeight = (int)cEntryPoint.viewport_height;
        ViewportDispatchZ = (int)cEntryPoint.viewport_dispatch_z;
        Samplers = Enumerable.Range(0, (int)cEntryPoint.samplers_size)
            .Select(i => new SamplerInfo(Marshal.ReadIntPtr(cEntryPoint.samplers + 8 * i)))
            .ToArray();
        Storages = Enumerable.Range(0, (int)cEntryPoint.storages_size)
            .Select(i => new StorageInfo(Marshal.ReadIntPtr(cEntryPoint.storages + 8 * i)))
            .ToArray();
    }
}
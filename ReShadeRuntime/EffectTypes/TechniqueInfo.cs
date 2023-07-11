using System.Runtime.InteropServices;
using Bindings.Metal;

namespace ReShadeRuntime.EffectTypes;

public class TechniqueInfo
{
    [StructLayout(LayoutKind.Sequential)]
    private struct CTechniqueInfo
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string name;
        public readonly nint passes;
        public readonly ulong passes_size;
        public readonly nint annotations;
        public readonly ulong annotations_size;
    }
    
    public string Name { get; }
    internal PassInfo[] Passes { get; }
    public Annotation[] Annotations { get; }
    public bool Enabled { get; set; }
    public int Priority { get; set; }
    private MTLDevice Device { get; set; }
    private IReadOnlyDictionary<string, MTLLibrary> Libraries { get; set; } = null!;
    private IReadOnlyList<UniformInfo> SpecConstants { get; set; } = null!;
    private IReadOnlyDictionary<string, MTLTexture> Textures { get; set; } = null!;
    private IReadOnlyDictionary<string, MTLSamplerState> Samplers { get; set; } = null!;
    private MTLTexture BackBuffer => Textures[Runtime.BackBufferKey];
    private List<MTLRenderPipelineState> PipelineStates { get; }

    public TechniqueInfo(nint cStruct, int priority)
    {
        var cEntryPoint = Marshal.PtrToStructure<CTechniqueInfo>(cStruct);
        Name = cEntryPoint.name;
        Enabled = true;
        Priority = priority;
        Passes = Enumerable.Range(0, (int)cEntryPoint.passes_size)
            .Select(i => new PassInfo(Marshal.ReadIntPtr(cEntryPoint.passes + 8 * i)))
            .ToArray();
        Annotations = Enumerable.Range(0, (int)cEntryPoint.annotations_size)
            .Select(i => new Annotation(Marshal.ReadIntPtr(cEntryPoint.annotations + 8 * i)))
            .ToArray();
        PipelineStates = new List<MTLRenderPipelineState>();
    }

    internal void PassRenderInfo(MTLDevice device, IReadOnlyDictionary<string, MTLLibrary> libraries,
        IReadOnlyList<UniformInfo> specConstants, IReadOnlyDictionary<string, MTLTexture> textures,
        IReadOnlyDictionary<string, MTLSamplerState> samplers)
    {
        Device = device;
        Libraries = libraries;
        SpecConstants = specConstants;
        Textures = textures;
        Samplers = samplers;
    }
    
    internal void UpdatePipelineStates()
    {
        MTLFunction? BuildFunction(string entryPoint, MTLFunctionConstantValues constants)
        {
            if (string.IsNullOrEmpty(entryPoint))
                return null;
            var library = Libraries[entryPoint];
            var libraryEntryPoint = library.FunctionNames.First();
            return library.newFunctionWithNameConstantValues(libraryEntryPoint, constants);
        }

        ReleasePipelineStates();
        
        foreach (var passInfo in Passes)
        {
            var constants = MTLFunctionConstantValues.New();
            for (var i = 0U; i < SpecConstants.Count; i++)
            {
                unsafe
                {
                    var specConstant = SpecConstants[(int)i];
                    fixed (int* value = specConstant.InitializerValue.AsInts)
                    {
                        constants.setConstantValuetypeatIndex(value, specConstant.Type.ToMTLDataType(),
                            new nuint(i));
                    }
                }
            }
            var vertexFunction = BuildFunction(passInfo.VertexShaderEntryPoint, constants);
            var fragmentFunction = BuildFunction(passInfo.FragmentShaderEntryPoint, constants);
            // var computeFunction = BuildFunction(passInfo.ComputeShaderEntryPoint, constants);
            constants.Release();
            var pipelineDescriptor = MTLRenderPipelineDescriptor.New();
            if (vertexFunction.HasValue) pipelineDescriptor.vertexFunction = vertexFunction.Value;
            if (fragmentFunction.HasValue) pipelineDescriptor.fragmentFunction = fragmentFunction.Value;
            // TODO: compute shaders are special

            var implicitBackBuffer = string.IsNullOrEmpty(passInfo.RenderTargetNames[0]);
            for (var i = 0; i < 8; i++)
            {
                if (string.IsNullOrEmpty(passInfo.RenderTargetNames[i]) && !implicitBackBuffer)
                    break;
                var texture = implicitBackBuffer ? BackBuffer : Textures[passInfo.RenderTargetNames[i]];
                var colorAttachment = pipelineDescriptor.colorAttachments[(uint)i];
                colorAttachment.blendingEnabled = new Bool8(passInfo.BlendEnable[i]);
                colorAttachment.pixelFormat = texture.pixelFormat;
                if (passInfo.BlendEnable[i] != 0)
                {
                    colorAttachment.blendingEnabled = new Bool8(true);
                    colorAttachment.rgbBlendOperation = PassInfo.ConvertToMTLBlendOperation(passInfo.BlendOp[i]);
                    colorAttachment.alphaBlendOperation = PassInfo.ConvertToMTLBlendOperation(passInfo.BlendOpAlpha[i]);
                    colorAttachment.sourceRGBBlendFactor = PassInfo.ConvertToMTLBlendFactor(passInfo.SrcBlend[i]);
                    colorAttachment.destinationRGBBlendFactor = PassInfo.ConvertToMTLBlendFactor(passInfo.DestBlend[i]);
                    colorAttachment.sourceAlphaBlendFactor = PassInfo.ConvertToMTLBlendFactor(passInfo.SrcBlendAlpha[i]);
                    colorAttachment.destinationAlphaBlendFactor = PassInfo.ConvertToMTLBlendFactor(passInfo.DestBlendAlpha[i]);
                }
                //colorAttachment.writeMask = (MTLColorWriteMask)passInfo.ColorWriteMask[i];
                implicitBackBuffer = false;
            }
            PipelineStates.Add(Device.newRenderPipelineStateWithDescriptor(pipelineDescriptor));
            vertexFunction?.Release();
            fragmentFunction?.Release();
            pipelineDescriptor.Release();
        }
    }

    private void ReleasePipelineStates()
    {
        foreach (var state in PipelineStates) state.Release();
        PipelineStates.Clear();
    }
    
    internal void Render(MTLCommandBuffer commandBuffer)
    {
        if (!Enabled) return;
        using var pipelineState = PipelineStates.GetEnumerator();
        foreach (var passInfo in Passes)
        {
            pipelineState.MoveNext();
            var descriptor = MTLRenderPassDescriptor.New();
            var colorAttachments = descriptor.colorAttachments;
            var implicitBackBuffer = string.IsNullOrEmpty(passInfo.RenderTargetNames[0]);
            for (var i = 0U; i < 8; i++)
            {
                if (string.IsNullOrEmpty(passInfo.RenderTargetNames[i]) && !implicitBackBuffer)
                    break;
                var texture = implicitBackBuffer ? BackBuffer : Textures[passInfo.RenderTargetNames[i]];
                var colorAttachment = colorAttachments[i];
                colorAttachment.texture = texture;
                colorAttachment.loadAction = passInfo.ClearRenderTargets == 0 ? MTLLoadAction.Load : MTLLoadAction.Clear;
                //colorAttachment.writeMask = (MTLColorWriteMask)passInfo.ColorWriteMask[i];
                implicitBackBuffer = false;
            }
            var commandEncoder = commandBuffer.renderCommandEncoderWithDescriptor(descriptor);
            commandEncoder.setRenderPipelineState(pipelineState.Current);
            for (var i = 0U; i < passInfo.Samplers.Length; i++)
            {
                commandEncoder.setFragmentTexture(Textures[passInfo.Samplers[i].TextureName], new nuint(i));
                commandEncoder.setFragmentSamplerState(Samplers[passInfo.Samplers[i].UniqueName], new nuint(i));
            }
            commandEncoder.drawPrimitives(passInfo.PrimitiveType, 0, new nuint(passInfo.NumVertices));
            commandEncoder.endEncoding();
        }
    }
}
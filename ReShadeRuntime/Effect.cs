using System.Runtime.InteropServices;
using Serilog;
using Bindings.Metal;
using ReShadeRuntime.EffectTypes;

namespace ReShadeRuntime;

internal class Effect
{
    [DllImport("libAetherium", CallingConvention = CallingConvention.Cdecl, EntryPoint = "reShadeLoadEffect")]
    private static extern nint UnmanagedLoadEffect(string sourceFile, int effectWidth, int effectHeight);

    private FileSystemInfo EffectFile { get; }

    private MTLDevice Device { get; }

    private Module Module { get; set; } = null!;

    private Dictionary<string, MTLTexture> Textures { get; }
    
    private MTLTexture BackBuffer => Textures[Runtime.BackBufferKey];

    private List<string> LoadedTextureNames { get; }

    private Dictionary<string, MTLSamplerState> Samplers { get; }

    private List<MTLRenderPipelineState> PipelineStates { get; set; }

    private Dictionary<string, MTLLibrary> Libraries { get; }

    private void LoadSamplers(IEnumerable<SamplerInfo> samplerInfos)
    {
        ReleaseSamplers();
        foreach (var samplerInfo in samplerInfos)
        {
            var descriptor = samplerInfo.ToDescriptor();
            Samplers[samplerInfo.UniqueName] = Device.newSamplerStateWithDescriptor(descriptor);
            descriptor.Release();
        }
    }
    
    private void ReleaseSamplers()
    {
        foreach (var sampler in Samplers.Values) sampler.Release();
        Samplers.Clear();
    }
    
    private void LoadTextures(IEnumerable<TextureInfo> textureInfos)
    {
        ReleaseTextures();
        foreach (var textureInfo in textureInfos)
        {
            var descriptor = textureInfo.ToDescriptor();
            if (Textures.ContainsKey(textureInfo.UniqueName)) continue;
            Textures[textureInfo.UniqueName] = Device.newTextureWithDescriptor(descriptor);
            descriptor.Release();
            LoadedTextureNames.Add(textureInfo.UniqueName);
        }
    }
    
    private void ReleaseTextures()
    {
        foreach (var loadedTextureName in LoadedTextureNames)
        {
            Textures.Remove(loadedTextureName, out var textureToRemove);
            textureToRemove.Release();
        }
        LoadedTextureNames.Clear();
    }
    
    private void LoadShaders(IEnumerable<EntryPoint> entryPoints)
    {
        ReleaseShaders();
        var compileOptions = MTLCompileOptions.New();
        foreach (var entryPoint in entryPoints)
            Libraries[entryPoint.Name] = Device.newLibraryWithSource(entryPoint.MslCode!, compileOptions);
        compileOptions.Release();
    }
    
    private void ReleaseShaders()
    {
        foreach (var library in Libraries.Values) 
            library.Release();
        Libraries.Clear();
    }
    
    private void UpdatePipelineStates(IEnumerable<TechniqueInfo> techniqueInfos)
    {
        MTLFunction? BuildFunction(string entryPoint, MTLFunctionConstantValues constants)
        {
            if (string.IsNullOrEmpty(entryPoint))
                return null;
            var library = Libraries[entryPoint];
            var libraryEntryPoint = library.FunctionNames.FirstOrDefault() ?? entryPoint;
            return library.newFunctionWithNameConstantValues(libraryEntryPoint, constants);
        }

        ReleasePipelineStates();
        
        foreach (var techniqueInfo in techniqueInfos)
        {
            foreach (var passInfo in techniqueInfo.Passes)
            {
                var constants = MTLFunctionConstantValues.New();
                for (var i = 0U; i < Module.SpecConstants.Length; i++)
                {
                    unsafe
                    {
                        var specConstant = Module.SpecConstants[i];
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
                        continue;
                    var texture = implicitBackBuffer ? BackBuffer : Textures[passInfo.RenderTargetNames[i]];
                    var colorAttachment = pipelineDescriptor.colorAttachments[(uint)i];
                    colorAttachment.blendingEnabled = new Bool8(passInfo.BlendEnable[i]);
                    colorAttachment.pixelFormat = texture.pixelFormat;
                    //colorAttachment.writeMask = (MTLColorWriteMask)passInfo.ColorWriteMask[i];
                    implicitBackBuffer = false;
                }
                PipelineStates.Add(Device.newRenderPipelineStateWithDescriptor(pipelineDescriptor));
                vertexFunction?.Release();
                fragmentFunction?.Release();
                pipelineDescriptor.Release();
            }
        }
    }

    private void ReleasePipelineStates()
    {
        foreach (var state in PipelineStates) state.Release();
        PipelineStates.Clear();
    }

    public Effect(MTLDevice device, FileSystemInfo effectFile, Dictionary<string, MTLTexture> textures, int effectWidth,
        int effectHeight)
    {
        EffectFile = effectFile;
        Device = device;
        Textures = textures;
        LoadedTextureNames = new List<string>();
        Samplers = new Dictionary<string, MTLSamplerState>();
        Libraries = new Dictionary<string, MTLLibrary>();
        PipelineStates = new List<MTLRenderPipelineState>();
        Reload(effectWidth, effectHeight);
    }

    public void Reload(int effectWidth, int effectHeight)
    {
        Module = new Module(UnmanagedLoadEffect(EffectFile.FullName, effectWidth, effectHeight));
        LoadSamplers(Module.Samplers);
        LoadTextures(Module.Textures);
        LoadShaders(Module.EntryPoints);
        UpdatePipelineStates(Module.Techniques);
    }

    public void Render(MTLCommandBuffer commandBuffer)
    {
        using var pipelineState = PipelineStates.GetEnumerator();
        foreach (var techniqueInfo in Module.Techniques)
        {
            foreach (var passInfo in techniqueInfo.Passes)
            {
                pipelineState.MoveNext();
                var descriptor = MTLRenderPassDescriptor.New();
                var colorAttachments = descriptor.colorAttachments;
                var implicitBackBuffer = string.IsNullOrEmpty(passInfo.RenderTargetNames[0]);
                for (var i = 0U; i < 8; i++)
                {
                    if (string.IsNullOrEmpty(passInfo.RenderTargetNames[i]) && !implicitBackBuffer)
                        continue;
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
}
using System.Runtime.InteropServices;
using Serilog;
using Bindings.Metal;
using ReShadeRuntime.EffectTypes;

namespace ReShadeRuntime;

internal partial class Effect
{
    [LibraryImport("libAetherium", EntryPoint = "reShadeLoadEffect", StringMarshalling = StringMarshalling.Utf8)]
    private static partial nint UnmanagedLoadEffect(string sourceFile, int effectWidth, int effectHeight);

    private FileSystemInfo EffectFile { get; }

    private MTLDevice Device { get; }

    public Module Module { get; private set; } = null!;

    private Dictionary<string, MTLTexture> Textures { get; }
    
    private MTLTexture BackBuffer => Textures[Runtime.BackBufferKey];

    private List<string> LoadedTextureNames { get; }

    private Dictionary<string, MTLSamplerState> Samplers { get; }

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
            var sampler = Module.Samplers.FirstOrDefault(sampler => sampler.TextureName == textureInfo.UniqueName);
            var srgb = (sampler?.Srgb ?? 0) != 0;
            var descriptor = textureInfo.ToDescriptor(srgb);
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

    public Effect(MTLDevice device, FileSystemInfo effectFile, Dictionary<string, MTLTexture> textures, int effectWidth,
        int effectHeight)
    {
        EffectFile = effectFile;
        Device = device;
        Textures = textures;
        LoadedTextureNames = new List<string>();
        Samplers = new Dictionary<string, MTLSamplerState>();
        Libraries = new Dictionary<string, MTLLibrary>();
        ReloadFromDisk(effectWidth, effectHeight);
    }
    
    public void ReloadFromDisk(int effectWidth, int effectHeight)
    {
        Module = new Module(UnmanagedLoadEffect(EffectFile.FullName, effectWidth, effectHeight));
        foreach (var technique in Module.Techniques) 
            technique.PassRenderInfo(Device, Libraries, Module.SpecConstants, Textures, Samplers);
        Reload();
    }

    public void Reload()
    {
        LoadSamplers(Module.Samplers);
        LoadTextures(Module.Textures);
        LoadShaders(Module.EntryPoints);
        foreach (var technique in Module.Techniques) 
            technique.UpdatePipelineStates();
    }
}
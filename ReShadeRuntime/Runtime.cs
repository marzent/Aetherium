using Bindings.Metal;

namespace ReShadeRuntime;

public class Runtime
{
    internal const string BackBufferKey = "V__ReShade__BackBufferTex";
    private const string DepthBufferKey = "V__ReShade__DepthBufferTex";
    private MTLDevice Device { get; }
    private List<Effect> Effects { get; }

    private MTLTexture? BackBuffer
    {
        get => Textures.TryGetValue(BackBufferKey, out var result) ? result : null;
        set => Textures[BackBufferKey] = value!.Value;
    }
    
    private MTLTexture DepthBuffer
    {
        set => Textures[DepthBufferKey] = value;
    }
    
    private int EffectWidth => (int)(BackBuffer?.Width ?? 800);
    private int EffectHeight => (int)(BackBuffer?.Height ?? 600);

    private Dictionary<string, MTLTexture> Textures { get; }

    public Runtime(MTLDevice device)
    {
        Device = device;
        Textures = new Dictionary<string, MTLTexture>();
        Effects = new List<Effect>();
    }

    public void AddEffect(FileSystemInfo effectFile)
    {
        Effects.Add(new Effect(Device, effectFile, Textures, EffectWidth, EffectHeight));
    }
    
    public void Reload()
    {
        foreach (var effect in Effects) 
            effect.Reload(EffectWidth, EffectHeight);
    }

    public void Render(MTLCommandBuffer commandBuffer, MTLTexture backBuffer, MTLTexture depthBuffer)
    {
        BackBuffer = backBuffer;
        DepthBuffer = depthBuffer;
        foreach (var effect in Effects) 
            effect.Render(commandBuffer);
    }
}
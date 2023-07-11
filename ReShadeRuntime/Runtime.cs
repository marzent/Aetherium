using Bindings.Metal;
using ReShadeRuntime.EffectTypes;

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

    public List<TechniqueInfo> Techniques { get; private set; }

    private Dictionary<string, MTLTexture> Textures { get; }

    public Runtime(MTLDevice device)
    {
        Device = device;
        Textures = new Dictionary<string, MTLTexture>();
        Effects = new List<Effect>();
        Techniques = new List<TechniqueInfo>();
    }

    public void AddEffect(FileSystemInfo effectFile)
    {
        Effects.Add(new Effect(Device, effectFile, Textures, EffectWidth, EffectHeight));
        OrderTechniques();
    }
    
    public void Reload()
    {
        foreach (var effect in Effects) 
            effect.ReloadFromDisk(EffectWidth, EffectHeight);
    }

    public void OrderTechniques()
    {
        Techniques = Effects.SelectMany(effect => effect.Module.Techniques)
            .OrderBy(tech => tech.Priority).ToList();
        for (var i = 0; i < Techniques.Count; i++) Techniques[i].Priority = i;
    }
        

    public void Render(MTLCommandBuffer commandBuffer, MTLTexture backBuffer, MTLTexture depthBuffer)
    {
        BackBuffer = backBuffer;
        DepthBuffer = depthBuffer;
        foreach (var technique in Techniques) 
            technique.Render(commandBuffer);
    }
}
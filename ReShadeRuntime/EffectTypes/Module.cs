using System.Runtime.InteropServices;
using Bindings.Metal;

namespace ReShadeRuntime.EffectTypes;

internal partial class Module
{
    [LibraryImport("libc", EntryPoint = "free")]
    private static partial void Free(nint pointer);
    
    [LibraryImport("libAetherium", EntryPoint = "freeModule")]
    private static partial void FreeCModule(nint module);
    
    [LibraryImport("libAetherium", EntryPoint = "spirvToMsl", StringMarshalling = StringMarshalling.Utf8)]
    private static partial nint SpirvToMsl(nint spirvData, long spirvSize, string entryPoint, EntryPoint.ShaderType type);

    [StructLayout(LayoutKind.Sequential)]
    private struct CModule
    {
        public readonly nint code;
        public readonly ulong code_size;
        public readonly nint entry_points;
        public readonly ulong entry_points_size;
        public readonly nint textures;
        public readonly ulong textures_size;
        public readonly nint samplers;
        public readonly ulong samplers_size;
        public readonly nint storages;
        public readonly ulong storages_size;
        public readonly nint uniforms;
        public readonly nint spec_constants;
        public readonly ulong uniforms_size;
        public readonly ulong spec_constants_size;
        public readonly nint techniques;
        public readonly ulong techniques_size;
        public readonly uint total_uniform_size;
        public readonly uint num_texture_bindings;
        public readonly uint num_sampler_bindings;
        public readonly uint num_storage_bindings;
    }
    
    public byte[] SpirvCode { get; }
    public EntryPoint[] EntryPoints { get; }
    public TextureInfo[] Textures { get; }
    public SamplerInfo[] Samplers { get; }
    public StorageInfo[] Storages { get; }
    public UniformInfo[] Uniforms { get; }
    public UniformInfo[] SpecConstants { get; }
    public TechniqueInfo[] Techniques { get; }
    public int TotalUniformSize { get; }
    public int NumTextureBindings { get; }
    public int NumSamplerBindings { get; }
    public int NumStorageBindings { get; }
    
    public Module(nint cStruct)
    {
        var cModule = Marshal.PtrToStructure<CModule>(cStruct);
        SpirvCode = new byte[cModule.code_size];
        Marshal.Copy(cModule.code, SpirvCode, 0, SpirvCode.Length);
        EntryPoints = Enumerable.Range(0, (int)cModule.entry_points_size)
            .Select(i => new EntryPoint(Marshal.ReadIntPtr(cModule.entry_points + 8 * i)))
            .ToArray();
        Textures = Enumerable.Range(0, (int)cModule.textures_size)
            .Select(i => new TextureInfo(Marshal.ReadIntPtr(cModule.textures + 8 * i)))
            .ToArray();
        Samplers = Enumerable.Range(0, (int)cModule.samplers_size)
            .Select(i => new SamplerInfo(Marshal.ReadIntPtr(cModule.samplers + 8 * i)))
            .ToArray();
        Storages = Enumerable.Range(0, (int)cModule.storages_size)
            .Select(i => new StorageInfo(Marshal.ReadIntPtr(cModule.storages + 8 * i)))
            .ToArray();
        Uniforms = Enumerable.Range(0, (int)cModule.uniforms_size)
            .Select(i => new UniformInfo(Marshal.ReadIntPtr(cModule.uniforms + 8 * i)))
            .ToArray();
        SpecConstants = Enumerable.Range(0, (int)cModule.spec_constants_size)
            .Select(i => new UniformInfo(Marshal.ReadIntPtr(cModule.spec_constants + 8 * i)))
            .ToArray();
        Techniques = Enumerable.Range(0, (int)cModule.techniques_size)
            .Select(i => new TechniqueInfo(Marshal.ReadIntPtr(cModule.techniques + 8 * i)))
            .ToArray();
        TotalUniformSize = (int)cModule.total_uniform_size;
        NumTextureBindings = (int)cModule.num_texture_bindings;
        NumSamplerBindings = (int)cModule.num_sampler_bindings;
        NumStorageBindings = (int)cModule.num_storage_bindings;

        foreach (var entryPoint in EntryPoints)
        {
            var cString = SpirvToMsl(cModule.code, (long)cModule.code_size, entryPoint.Name, entryPoint.Type);
            if (cString == nint.Zero)
                throw new NullReferenceException("Shader conversion to MSL failed");
            entryPoint.MslCode = Marshal.PtrToStringAuto(cString)!;
            Free(cString);
        }
        
        FreeCModule(cStruct);
    }
}
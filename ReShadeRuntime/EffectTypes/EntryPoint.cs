using System.Runtime.InteropServices;

namespace ReShadeRuntime.EffectTypes;

internal class EntryPoint
{
    public enum ShaderType
    {
        Vertex,
        Fragment,
        Compute
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct CEntryPoint
    {
        [MarshalAs(UnmanagedType.LPStr)] 
        public readonly string name;
        public readonly ShaderType type;
    }
    
    public string Name { get; }
    public ShaderType Type { get; }
    public string? MslCode { get; set; }

    public EntryPoint(nint cStruct)
    {
        var cEntryPoint = Marshal.PtrToStructure<CEntryPoint>(cStruct);
        Name = cEntryPoint.name;
        Type = cEntryPoint.type;
    }
}
using System.Runtime.InteropServices;

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

    public TechniqueInfo(nint cStruct)
    {
        var cEntryPoint = Marshal.PtrToStructure<CTechniqueInfo>(cStruct);
        Name = cEntryPoint.name;
        Enabled = true;
        Priority = 0;
        Passes = Enumerable.Range(0, (int)cEntryPoint.passes_size)
            .Select(i => new PassInfo(Marshal.ReadIntPtr(cEntryPoint.passes + 8 * i)))
            .ToArray();
        Annotations = Enumerable.Range(0, (int)cEntryPoint.annotations_size)
            .Select(i => new Annotation(Marshal.ReadIntPtr(cEntryPoint.annotations + 8 * i)))
            .ToArray();
    }
}
using System.Runtime.InteropServices;

namespace ReShadeRuntime.EffectTypes;

internal class TechniqueInfo
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
    public PassInfo[] Passes { get; }
    public Annotation[] Annotations { get; }

    public TechniqueInfo(nint cStruct)
    {
        var cEntryPoint = Marshal.PtrToStructure<CTechniqueInfo>(cStruct);
        Name = cEntryPoint.name;
        Passes = Enumerable.Range(0, (int)cEntryPoint.passes_size)
            .Select(i => new PassInfo(cEntryPoint.passes + 8 * i))
            .ToArray();
        Annotations = Enumerable.Range(0, (int)cEntryPoint.annotations_size)
            .Select(i => new Annotation(cEntryPoint.annotations + 8 * i))
            .ToArray();
    }
}
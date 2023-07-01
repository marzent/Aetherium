using System.Runtime.InteropServices;

namespace ReShadeRuntime.EffectTypes;

internal class UniformInfo
{
    [StructLayout(LayoutKind.Sequential)]
    private struct CUniformInfo
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string name;
        public nint type;
        public uint size;
        public uint offset;
        public nint annotations;
        public ulong annotations_size;
        [MarshalAs(UnmanagedType.I1)]
        public bool has_initializer_value;
        public nint initializer_value;
    }
    
    public string Name { get; }
    public Type Type { get; }
    public uint Size { get; }
    public uint Offset { get; }
    public Annotation[] Annotations { get; }
    public bool HasInitializerValue { get; }
    public Constant InitializerValue { get; }

    public UniformInfo(nint cStruct)
    {
        var cUniformInfo = Marshal.PtrToStructure<CUniformInfo>(cStruct);
        Name = cUniformInfo.name;
        Type = new Type(cUniformInfo.type);
        Size = cUniformInfo.size;
        Offset = cUniformInfo.offset;
        Annotations = Enumerable.Range(0, (int)cUniformInfo.annotations_size)
            .Select(i => new Annotation(Marshal.ReadIntPtr(cUniformInfo.annotations + 8 * i)))
            .ToArray();
        HasInitializerValue = cUniformInfo.has_initializer_value;
        InitializerValue = new Constant(cUniformInfo.initializer_value);
    }
}
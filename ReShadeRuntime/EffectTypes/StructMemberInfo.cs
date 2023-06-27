using System.Runtime.InteropServices;

namespace ReShadeRuntime.EffectTypes;

internal class StructMemberInfo
{
    [StructLayout(LayoutKind.Sequential)]
    private struct CStructMemberInfo
    {
        public readonly nint type;
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string name;
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string semantic;
        public readonly uint definition;
    }

    public Type Type { get; }
    public string Name { get; }
    public string Semantic { get; }
    public int Definition { get; }

    public StructMemberInfo(nint cStruct)
    {
        var cStructMemberInfo = Marshal.PtrToStructure<CStructMemberInfo>(cStruct);
        Type = new Type(cStructMemberInfo.type);
        Name = cStructMemberInfo.name;
        Semantic = cStructMemberInfo.semantic;
        Definition = (int)cStructMemberInfo.definition;
    }
    
}
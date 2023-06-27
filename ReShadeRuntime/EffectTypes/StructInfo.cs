using System.Runtime.InteropServices;

namespace ReShadeRuntime.EffectTypes;

internal class StructInfo
{
    [StructLayout(LayoutKind.Sequential)]
    private struct CStructInfo
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string name;
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string unique_name;
        public readonly nint member_list;
        public readonly ulong member_list_size;
        public readonly uint definition;
    }
    
    public string Name { get; }
    public string UniqueName { get; }
    public StructMemberInfo[] MemberList { get; }
    public int Definition { get; }

    public StructInfo(nint cStruct)
    {
        var cStructInfo = Marshal.PtrToStructure<CStructInfo>(cStruct);
        Name = cStructInfo.name;
        UniqueName = cStructInfo.unique_name;
        Definition = (int)cStructInfo.definition;
        MemberList = Enumerable.Range(0, (int)cStructInfo.member_list_size)
            .Select(i => new StructMemberInfo(cStructInfo.member_list + 8 * i))
            .ToArray();
    }
    
}
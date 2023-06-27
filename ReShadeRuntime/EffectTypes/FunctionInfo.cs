using System.Runtime.InteropServices;

namespace ReShadeRuntime.EffectTypes;

internal class FunctionInfo
{
    [StructLayout(LayoutKind.Sequential)]
    private struct CFunctionInfo
    {
        public uint definition;
        [MarshalAs(UnmanagedType.LPStr)]
        public string name;
        [MarshalAs(UnmanagedType.LPStr)]
        public string unique_name;
        public nint return_type;
        [MarshalAs(UnmanagedType.LPStr)]
        public string return_semantic;
        public nint parameter_list;
        public ulong parameter_list_size;
        public nint referenced_samplers;
        public ulong referenced_samplers_size;
        public nint referenced_storages;
        public ulong referenced_storages_size;
    }
    
    public uint Definition { get; }
    public string Name { get; }
    public string UniqueName { get; }
    public Type ReturnType { get; }
    public string ReturnSemantic { get; }
    public HashSet<int> ReferencedSamplerIds { get; }
    public HashSet<int> ReferencedStorageIds { get; }
    
    public FunctionInfo(nint cStruct)
    {
        var cEntryPoint = Marshal.PtrToStructure<CFunctionInfo>(cStruct);
        Definition = cEntryPoint.definition;
        Name = cEntryPoint.name;
        UniqueName = cEntryPoint.unique_name;
        ReturnType = new Type(cEntryPoint.return_type);
        ReturnSemantic = cEntryPoint.return_semantic;
        var samplerIds = new int[cEntryPoint.referenced_samplers_size];
        Marshal.Copy(cEntryPoint.referenced_samplers, samplerIds, 0, samplerIds.Length);
        ReferencedSamplerIds = new HashSet<int>(samplerIds);
        var storageIds = new int[cEntryPoint.referenced_storages_size];
        Marshal.Copy(cEntryPoint.referenced_storages, storageIds, 0, storageIds.Length);
        ReferencedStorageIds = new HashSet<int>(storageIds);
    }
}
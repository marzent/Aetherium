using System.Runtime.InteropServices;

namespace ReShadeRuntime.EffectTypes;

internal class StorageInfo
{
    [StructLayout(LayoutKind.Sequential)]
    private struct CStorageInfo
    {
        public uint id;
        public uint binding;
        [MarshalAs(UnmanagedType.LPStr)]
        public string name;
        [MarshalAs(UnmanagedType.LPStr)]
        public string unique_name;
        [MarshalAs(UnmanagedType.LPStr)]
        public string texture_name;
        public TextureInfo.TextureFormat format;
        public ushort level;
    }
    
    public int Id { get; }
    public int Binding { get; }
    public string Name { get; }
    public string UniqueName { get; }
    public string TextureName { get; }
    public TextureInfo.TextureFormat Format { get; }
    public ushort Level { get; }

    public StorageInfo(nint cStruct)
    {
        var cStorageInfo = Marshal.PtrToStructure<CStorageInfo>(cStruct);
        Id = (int)cStorageInfo.id;
        Binding = (int)cStorageInfo.binding;
        Name = cStorageInfo.name;
        UniqueName = cStorageInfo.unique_name;
        TextureName = cStorageInfo.texture_name;
        Format = cStorageInfo.format;
        Level = cStorageInfo.level;
    }
}
using System.Runtime.InteropServices;

namespace ReShadeRuntime.EffectTypes;

public class Constant
{
    [StructLayout(LayoutKind.Explicit)]
    private struct CConstant
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public readonly float[] as_float;
    
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public readonly int[] as_int;
    
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public readonly uint[] as_uint;

        [FieldOffset(64)]
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string string_data;
    
        [FieldOffset(72)]
        public readonly nint array_data;

        [FieldOffset(80)]
        public readonly ulong array_size;
    }

    public float[] AsFloats { get; }
    public int[] AsInts { get; }
    public uint[] AsUints { get; }
    public string StringData { get; }
    public Constant[] ArrayData { get; }

    public Constant(nint cStruct)
    {
        var cConstant = Marshal.PtrToStructure<CConstant>(cStruct);
        AsFloats = cConstant.as_float;
        AsInts = cConstant.as_int;
        AsUints = cConstant.as_uint;
        StringData = cConstant.string_data;
        ArrayData = Enumerable.Range(0, (int)cConstant.array_size)
            .Select(i => new Constant(Marshal.ReadIntPtr(cConstant.array_data + 8 * i)))
            .ToArray();
    }
}
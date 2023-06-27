using System.Runtime.InteropServices;

namespace ReShadeRuntime.EffectTypes;

internal class Constant
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

        [FieldOffset(16)]
        [MarshalAs(UnmanagedType.LPStr)]
        public readonly string string_data;
    
        [FieldOffset(24)]
        public readonly nint array_data;

        [FieldOffset(32)]
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
            .Select(i => new Constant(cConstant.array_data + 8 * i))
            .ToArray();
    }
}
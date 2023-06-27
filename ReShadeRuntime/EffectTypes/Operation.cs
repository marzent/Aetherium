using System.Runtime.InteropServices;

namespace ReShadeRuntime.EffectTypes;

internal class Operation
{
    public enum OperationType
    {
        OpCast,
        OpMember,
        OpDynamicIndex,
        OpConstantIndex,
        OpSwizzle
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct COperation
    {
        public OperationType op;
        public nint from;
        public nint to;
        public uint index;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public sbyte[] swizzle;
    }

    public OperationType Op { get; }
    public Type From { get; }
    public Type To { get; }
    public int Index { get; }
    public sbyte[] Swizzle { get; }

    public Operation(nint cStruct)
    {
        var cOperation = Marshal.PtrToStructure<COperation>(cStruct);
        Op = cOperation.op;
        From = new Type(cOperation.from);
        To = new Type(cOperation.to);
        Index = (int)cOperation.index;
        Swizzle = cOperation.swizzle;
    }
    
}
using System.Runtime.InteropServices;

namespace ReShadeRuntime.EffectTypes;

internal class Expression
{
    [StructLayout(LayoutKind.Sequential)]
    private struct CExpression
    {
        public readonly uint _base;
        public readonly nint expression_type;
        public readonly nint constant_value;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool is_lvalue;
        [MarshalAs(UnmanagedType.I1)]
        public readonly bool is_constant;
        public readonly nint operations_chain;
        public readonly ulong operations_chain_size;
    }

    public uint Base { get; }
    public Type ExpressionType { get; }
    public Constant ConstantValue { get; }
    public bool isLvalue { get; }
    public bool isConstant { get; }
    public Operation[] Operations { get; }

    public Expression(nint cStruct)
    {
        var cExpression = Marshal.PtrToStructure<CExpression>(cStruct);
        Base = cExpression._base;
        ExpressionType = new Type(cExpression.expression_type);
        ConstantValue = new Constant(cExpression.constant_value);
        isLvalue = cExpression.is_lvalue;
        isConstant = cExpression.is_constant;
        Operations = Enumerable.Range(0, (int)cExpression.operations_chain_size)
            .Select(i => new Operation(cExpression.operations_chain + 8 * i))
            .ToArray();
    }
    
}
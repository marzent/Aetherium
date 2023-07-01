using System.Runtime.InteropServices;
using Bindings.Metal;

namespace ReShadeRuntime.EffectTypes;

public class Type
{
    public enum DataType
    {
        TVoid,
        TBool,
        TMin16Int,
        TInt,
        TMin16Uint,
        TUint,
        TMin16Float,
        TFloat,
        TString,
        TStruct,
        TSampler,
        TStorage,
        TTexture,
        TFunction
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct CType
    {
        public readonly DataType dataType;
        public readonly uint rows;
        public readonly uint cols;
        public readonly uint qualifiers;
        public readonly int array_length;
        public readonly uint definition;
        [MarshalAs(UnmanagedType.LPStr)] public readonly string description;
    }

    public DataType UnderlyingType { get; }
    public uint Rows { get; }
    public uint Cols { get; }
    public uint Qualifiers { get; }
    public int ArrayLength { get; }
    public uint Definition { get; }
    public string Description { get; }

    public Type(nint cStruct)
    {
        var cType = Marshal.PtrToStructure<CType>(cStruct);
        UnderlyingType = cType.dataType;
        Rows = cType.rows;
        Cols = cType.cols;
        Qualifiers = cType.qualifiers;
        ArrayLength = cType.array_length;
        Definition = cType.definition;
        Description = cType.description;
    }

    public MTLDataType ToMTLDataType()
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return UnderlyingType switch
        {
            DataType.TVoid => MTLDataType.None,
            DataType.TBool => MTLDataType.Bool,
            DataType.TMin16Int => MTLDataType.Short,
            DataType.TInt => MTLDataType.Int,
            DataType.TMin16Uint => MTLDataType.UShort,
            DataType.TUint => MTLDataType.UInt,
            DataType.TMin16Float => MTLDataType.Half,
            DataType.TFloat => MTLDataType.Float,
            DataType.TStruct => MTLDataType.Struct,
            DataType.TSampler => MTLDataType.Sampler,
            DataType.TStorage => MTLDataType.Texture,
            DataType.TTexture => MTLDataType.Texture,
            _ => throw new ArgumentOutOfRangeException(nameof(UnderlyingType),
                $"Unsupported data type: {UnderlyingType}")
        };
    }
}
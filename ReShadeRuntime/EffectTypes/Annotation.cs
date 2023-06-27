using System.Runtime.InteropServices;

namespace ReShadeRuntime.EffectTypes;

internal class Annotation
{
    [StructLayout(LayoutKind.Sequential)]
    private struct CAnnotation
    {
        public readonly nint type;
        [MarshalAs(UnmanagedType.LPStr)] 
        public readonly string name;
        public readonly nint value;
    }
    
    public Type Type { get; }
    public string Name { get; }
    public Constant Value { get; }

    public Annotation(nint cStruct)
    {
        var cAnnotation = Marshal.PtrToStructure<CAnnotation>(cStruct);
        Type = new Type(cAnnotation.type);
        Name = cAnnotation.name;
        Value = new Constant(cAnnotation.value);
    }
    
}
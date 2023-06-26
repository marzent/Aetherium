namespace Bindings.Metal;

// TODO: Technically this should be "pointer-sized",
// but there are no non-64-bit platforms that anyone cares about.
public struct CGFloat
{
    private readonly double _value;

    public CGFloat(double value)
    {
        _value = value;
    }

    public double Value
    {
        get => _value;
    }

    public static implicit operator CGFloat(double value) => new CGFloat(value);
    public static implicit operator double(CGFloat cgf) => cgf.Value;
    public static implicit operator float(CGFloat cgf) => (float)cgf.Value;

    public override string ToString() => _value.ToString();
}
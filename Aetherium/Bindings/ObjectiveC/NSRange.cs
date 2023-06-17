namespace Aetherium.Bindings.ObjectiveC;

public struct NSRange
{
    public nuint location;
    public nuint length;

    public NSRange(nuint location, nuint length)
    {
        this.location = location;
        this.length = length;
    }

    public NSRange(uint location, uint length)
    {
        this.location = (nuint)location;
        this.length = (nuint)length;
    }
}
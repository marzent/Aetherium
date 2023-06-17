namespace Aetherium.Bindings.Metal;

public struct MTLOrigin
{
    public nuint x;
    public nuint y;
    public nuint z;

    public MTLOrigin(uint x, uint y, uint z)
    {
        this.x = (nuint)x;
        this.y = (nuint)y;
        this.z = (nuint)z;
    }
}
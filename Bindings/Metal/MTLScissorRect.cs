namespace Bindings.Metal;

public struct MTLScissorRect
{
    public nuint x;
    public nuint y;
    public nuint width;
    public nuint height;

    public MTLScissorRect(uint x, uint y, uint width, uint height)
    {
        this.x = (nuint)x;
        this.y = (nuint)y;
        this.width = (nuint)width;
        this.height = (nuint)height;
    }
}
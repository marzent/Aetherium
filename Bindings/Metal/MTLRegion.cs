namespace Bindings.Metal;

public struct MTLRegion
{
    public MTLOrigin origin;
    public MTLSize size;

    public MTLRegion(MTLOrigin origin, MTLSize size)
    {
        this.origin = origin;
        this.size = size;
    }
}
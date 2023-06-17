using System.Runtime.InteropServices;

namespace Aetherium.Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLSize
{
    public nuint Width;
    public nuint Height;
    public nuint Depth;

    public MTLSize(uint width, uint height, uint depth)
    {
        Width = (nuint)width;
        Height = (nuint)height;
        Depth = (nuint)depth;
    }
}
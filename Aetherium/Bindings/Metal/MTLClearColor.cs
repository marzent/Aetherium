using System.Runtime.InteropServices;

namespace Aetherium.Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLClearColor
{
    public double red;
    public double green;
    public double blue;
    public double alpha;

    public MTLClearColor(double r, double g, double b, double a)
    {
        red = r;
        green = g;
        blue = b;
        alpha = a;
    }
}
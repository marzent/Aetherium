namespace Aetherium.Bindings.Metal;

public unsafe struct BlockLiteral
{
    public nint isa;
    public int flags;
    public int reserved;
    public nint invoke;
    public BlockDescriptor* descriptor;
};

public struct BlockDescriptor
{
    public ulong reserved;
    public ulong Block_size;
}
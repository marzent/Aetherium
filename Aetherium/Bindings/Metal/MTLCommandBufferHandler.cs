using System.Runtime.InteropServices;

namespace Aetherium.Bindings.Metal;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MTLCommandBufferHandler(nint block, MTLCommandBuffer buffer);
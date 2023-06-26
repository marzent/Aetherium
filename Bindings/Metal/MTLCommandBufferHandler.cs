using System.Runtime.InteropServices;

namespace Bindings.Metal;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void MTLCommandBufferHandler(nint block, MTLCommandBuffer buffer);
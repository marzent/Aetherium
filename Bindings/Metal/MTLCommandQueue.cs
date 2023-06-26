using System.Runtime.InteropServices;
using Bindings.ObjectiveC;
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLCommandQueue
{
    public readonly nint NativePtr;

    public MTLCommandBuffer commandBuffer() => objc_msgSend<MTLCommandBuffer>(NativePtr, sel_commandBuffer);

    public void insertDebugCaptureBoundary() => objc_msgSend(NativePtr, sel_insertDebugCaptureBoundary);

    private static readonly Selector sel_commandBuffer = "commandBuffer";
    private static readonly Selector sel_insertDebugCaptureBoundary = "insertDebugCaptureBoundary";
}
using static Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Bindings.ObjectiveC;

public struct NSApplication
{
    public readonly nint NativePtr;
    public NSApplication(nint ptr) => NativePtr = ptr;
    
    public NSWindow mainWindow => objc_msgSend<NSWindow>(NativePtr, "mainWindow");
    
    public NSArray windows => objc_msgSend<NSArray>(NativePtr, "windows");
    
    public static NSApplication shared => new(s_class.GetProperty("sharedApplication"));
    
    private static readonly ObjCClass s_class = new(nameof(NSApplication));
}
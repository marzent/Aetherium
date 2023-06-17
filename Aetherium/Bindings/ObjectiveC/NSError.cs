using static Aetherium.Bindings.ObjectiveC.ObjectiveCRuntime;

namespace Aetherium.Bindings.ObjectiveC;

public struct NSError
{
    public readonly nint NativePtr;
    public string domain => string_objc_msgSend(NativePtr, "domain");
    public string localizedDescription => string_objc_msgSend(NativePtr, "localizedDescription");
}
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Bindings.Metal;

namespace Bindings.ObjectiveC;

public static unsafe class ObjectiveCRuntime
{
    private const string ObjCLibrary = "/usr/lib/libobjc.A.dylib";

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, float a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, double a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, CGRect a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, nint a, uint b);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, nint a, NSRange b);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, MTLSize a, MTLSize b);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, nint c, nuint d, MTLSize e);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, MTLClearColor a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, CGSize a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, nint a, nuint b, nuint c);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, void* a, nuint b, nuint c);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, MTLPrimitiveType a, nuint b, nuint c, nuint d, nuint e);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, MTLPrimitiveType a, nuint b, nuint c, nuint d);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, MTLPrimitiveType a, nuint b, nuint c);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, NSRange a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, nuint a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, MTLCommandBufferHandler a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, nint a, nuint b);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, MTLViewport a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, MTLScissorRect a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, void* a, uint b, nuint c);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, void* a, nuint b);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, MTLPrimitiveType a, nuint b, MTLIndexType c, nint d, nuint e, nuint f);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, MTLPrimitiveType a, MTLBuffer b, nuint c);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(
        nint receiver,
        Selector selector,
        MTLPrimitiveType a,
        nuint b,
        MTLIndexType c,
        nint d,
        nuint e,
        nuint f,
        nint g,
        nuint h);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(
        nint receiver,
        Selector selector,
        MTLPrimitiveType a,
        MTLIndexType b,
        MTLBuffer c,
        nuint d,
        MTLBuffer e,
        nuint f);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(
        nint receiver,
        Selector selector,
        MTLBuffer a,
        nuint b,
        MTLBuffer c,
        nuint d,
        nuint e);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(
        nint receiver,
        Selector selector,
        nint a,
        nuint b,
        nuint c,
        nuint d,
        MTLSize e,
        nint f,
        nuint g,
        nuint h,
        MTLOrigin i);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(
        nint receiver,
        Selector selector,
        MTLRegion a,
        nuint b,
        nuint c,
        nint d,
        nuint e,
        nuint f);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(
        nint receiver,
        Selector selector,
        MTLTexture a,
        nuint b,
        nuint c,
        MTLOrigin d,
        MTLSize e,
        MTLBuffer f,
        nuint g,
        nuint h,
        nuint i);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(
        nint receiver,
        Selector selector,
        MTLTexture sourceTexture,
        nuint sourceSlice,
        nuint sourceLevel,
        MTLOrigin sourceOrigin,
        MTLSize sourceSize,
        MTLTexture destinationTexture,
        nuint destinationSlice,
        nuint destinationLevel,
        MTLOrigin destinationOrigin);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern byte* bytePtr_objc_msgSend(nint receiver, Selector selector);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern CGSize CGSize_objc_msgSend(nint receiver, Selector selector);


    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern byte byte_objc_msgSend(nint receiver, Selector selector);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern Bool8 bool8_objc_msgSend(nint receiver, Selector selector);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern Bool8 bool8_objc_msgSend(nint receiver, Selector selector, nuint a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern Bool8 bool8_objc_msgSend(nint receiver, Selector selector, nint a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern Bool8 bool8_objc_msgSend(nint receiver, Selector selector, nuint a, nint b);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern Bool8 bool8_objc_msgSend(nint receiver, Selector selector, uint a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern uint uint_objc_msgSend(nint receiver, Selector selector);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern ulong ulong_objc_msgSend(nint receiver, Selector selector);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern float float_objc_msgSend(nint receiver, Selector selector);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]

    public static extern CGFloat CGFloat_objc_msgSend(nint receiver, Selector selector);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern double double_objc_msgSend(nint receiver, Selector selector);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nint IntPtr_objc_msgSend(nint receiver, Selector selector);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nint IntPtr_objc_msgSend(nint receiver, Selector selector, nint a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nint IntPtr_objc_msgSend(nint receiver, Selector selector, nint a, out NSError error);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nint IntPtr_objc_msgSend(nint receiver, Selector selector, uint a, uint b, NSRange c, NSRange d);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nint IntPtr_objc_msgSend(nint receiver, Selector selector, MTLComputePipelineDescriptor a, uint b, nint c, out NSError error);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nint IntPtr_objc_msgSend(nint receiver, Selector selector, MTLFunction f, out NSError error);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nint IntPtr_objc_msgSend(nint receiver, Selector selector, uint a);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nint IntPtr_objc_msgSend(nint receiver, Selector selector, nuint a);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nint IntPtr_objc_msgSend(nint receiver, Selector selector, nint a, nint b, out NSError error);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nint IntPtr_objc_msgSend(nint receiver, Selector selector, nint a, nuint b);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nint IntPtr_objc_msgSend(nint receiver, Selector selector, nuint b, MTLResourceOptions c);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nint IntPtr_objc_msgSend(nint receiver, Selector selector, void* a, nuint b, MTLResourceOptions c);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern nuint UIntPtr_objc_msgSend(nint receiver, Selector selector);

    public static T objc_msgSend<T>(nint receiver, Selector selector) where T : struct
    {
        nint value = IntPtr_objc_msgSend(receiver, selector);
        return Unsafe.AsRef<T>(&value);
    }
    public static T objc_msgSend<T>(nint receiver, Selector selector, nint a) where T : struct
    {
        nint value = IntPtr_objc_msgSend(receiver, selector, a);
        return Unsafe.AsRef<T>(&value);
    }
    public static string string_objc_msgSend(nint receiver, Selector selector)
    {
        return objc_msgSend<NSString>(receiver, selector).GetValue();
    }

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, byte b);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, Bool8 b);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, uint b);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, float a, float b, float c, float d);
    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern void objc_msgSend(nint receiver, Selector selector, nint b);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend_stret")]
    public static extern void objc_msgSend_stret(void* retPtr, nint receiver, Selector selector);
    public static T objc_msgSend_stret<T>(nint receiver, Selector selector) where T : struct
    {
        T ret = default(T);
        objc_msgSend_stret(Unsafe.AsPointer(ref ret), receiver, selector);
        return ret;
    }

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern MTLClearColor MTLClearColor_objc_msgSend(nint receiver, Selector selector);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern MTLSize MTLSize_objc_msgSend(nint receiver, Selector selector);

    [DllImport(ObjCLibrary, EntryPoint = "objc_msgSend")]
    public static extern CGRect CGRect_objc_msgSend(nint receiver, Selector selector);

    // TODO: This should check the current processor type, struct size, etc.
    // At the moment there is no need because all existing occurences of
    // this can safely use the non-stret versions everywhere.
    public static bool UseStret<T>() => false;

    [DllImport(ObjCLibrary)]
    public static extern nint sel_registerName(byte* namePtr);

    [DllImport(ObjCLibrary)]
    public static extern byte* sel_getName(nint selector);

    [DllImport(ObjCLibrary)]
    public static extern nint objc_getClass(byte* namePtr);

    [DllImport(ObjCLibrary)]
    public static extern ObjCClass object_getClass(nint obj);
    
    [DllImport(ObjCLibrary)]
    public static extern ObjectiveCMethod class_getInstanceMethod(ObjCClass cls, Selector name);
    
    [DllImport(ObjCLibrary)]
    public static extern ObjectiveCMethod class_getClassMethod(ObjCClass cls, Selector name);
    
    [DllImport(ObjCLibrary)]
    public static extern ObjectiveCMethod method_getImplementation(ObjectiveCMethod method);
    
    [DllImport(ObjCLibrary)]
    public static extern ObjectiveCMethod method_setImplementation(ObjectiveCMethod method, nint imp);

    [DllImport(ObjCLibrary)]
    public static extern nint class_getProperty(ObjCClass cls, byte* namePtr);

    [DllImport(ObjCLibrary)]
    public static extern byte* class_getName(ObjCClass cls);

    [DllImport(ObjCLibrary)]
    public static extern byte* property_copyAttributeValue(nint property, byte* attributeNamePtr);

    [DllImport(ObjCLibrary)]
    public static extern Selector method_getName(ObjectiveCMethod method);

    [DllImport(ObjCLibrary)]
    public static extern ObjectiveCMethod* class_copyMethodList(ObjCClass cls, out uint outCount);

    [DllImport(ObjCLibrary)]
    public static extern void free(nint receiver);
    public static void retain(nint receiver) => objc_msgSend(receiver, "retain");
    public static void release(nint receiver) => objc_msgSend(receiver, "release");
    public static ulong GetRetainCount(nint receiver) => (ulong)UIntPtr_objc_msgSend(receiver, "retainCount");
}
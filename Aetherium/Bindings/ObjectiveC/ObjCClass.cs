using System.Runtime.CompilerServices;
using System.Text;
using Aetherium.Bindings.Metal;

namespace Aetherium.Bindings.ObjectiveC;

public unsafe struct ObjCClass
{
    public readonly nint NativePtr;
    public static implicit operator nint(ObjCClass c) => c.NativePtr;

    public ObjCClass(string name)
    {
        int byteCount = Encoding.UTF8.GetMaxByteCount(name.Length);
        byte* utf8BytesPtr = stackalloc byte[byteCount];
        fixed (char* namePtr = name)
        {
            Encoding.UTF8.GetBytes(namePtr, name.Length, utf8BytesPtr, byteCount);
        }

        NativePtr = ObjectiveCRuntime.objc_getClass(utf8BytesPtr);
    }

    public nint GetProperty(string propertyName)
    {
        int byteCount = Encoding.UTF8.GetMaxByteCount(propertyName.Length);
        byte* utf8BytesPtr = stackalloc byte[byteCount];
        fixed (char* namePtr = propertyName)
        {
            Encoding.UTF8.GetBytes(namePtr, propertyName.Length, utf8BytesPtr, byteCount);
        }

        return ObjectiveCRuntime.class_getProperty(this, utf8BytesPtr);
    }
    
    public ObjectiveCMethod GetInstanceMethod(Selector selector) 
        => ObjectiveCRuntime.class_getInstanceMethod(this, selector);
    
    public ObjectiveCMethod GetClassMethod(Selector selector) 
        => ObjectiveCRuntime.class_getClassMethod(this, selector);

    public string Name => MTLUtil.GetUtf8String(ObjectiveCRuntime.class_getName(this));

    public T Alloc<T>() where T : struct
    {
        nint value = ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, Selectors.alloc);
        return Unsafe.AsRef<T>(&value);
    }

    public T AllocInit<T>() where T : struct
    {
        nint value = ObjectiveCRuntime.IntPtr_objc_msgSend(NativePtr, Selectors.alloc);
        ObjectiveCRuntime.objc_msgSend(value, Selectors.init);
        return Unsafe.AsRef<T>(&value);
    }

    public ObjectiveCMethod* class_copyMethodList(out uint count)
    {
        return ObjectiveCRuntime.class_copyMethodList(this, out count);
    }
}
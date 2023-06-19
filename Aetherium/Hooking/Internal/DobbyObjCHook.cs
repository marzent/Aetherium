using System;
using System.Reflection;
using Aetherium.Bindings.ObjectiveC;

namespace Aetherium.Hooking.Internal;

/// <summary>
/// Class facilitating hooks via Dobby.
/// </summary>
/// <typeparam name="T">Delegate of the hook.</typeparam>
internal class DobbyObjCHook<T> : DobbyHook<T> where T : Delegate
{
    private readonly string _className;
    private readonly string _methodName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DobbyObjCHook{T}"/> class.
    /// </summary>
    /// <param name="className">Name of the class containing the method to hook.</param>
    /// <param name="methodName">Name of the function.</param>
    /// <param name="detour">Callback function. Delegate must have a same original function prototype.</param>
    /// <param name="callingAssembly">Calling assembly.</param>
    internal DobbyObjCHook(string className, string methodName, T detour, Assembly callingAssembly)
        : base(GetMethodAddress(className, methodName), detour, callingAssembly)
    {
        _className = className;
        _methodName = methodName;
    }

    private static nint GetMethodAddress(string className, string methodName)
    {
        var objCClass = new ObjCClass(className);
        var method = objCClass.GetInstanceMethod(methodName);
        if (method == nint.Zero)
            method = objCClass.GetClassMethod(methodName);
        if (method == nint.Zero)
            throw new MethodAccessException($"Could not find [{className} {methodName}] implementation");
        return method.Implementation;
    }
}

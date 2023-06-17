using System;

namespace Aetherium.IoC;

/// <summary>
/// This attribute indicates whether the decorated class should be exposed to plugins via IoC.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PluginInterfaceAttribute : Attribute
{
}

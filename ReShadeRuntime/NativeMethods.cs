using System.Runtime.InteropServices;

namespace ReShadeRuntime;

public static class NativeMethods
{
    [DllImport("libAetherium", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern void showAlert(string message, string info, string buttonTitle);
}
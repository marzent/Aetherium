using System;
using System.Runtime.InteropServices;

namespace Aetherium.Hooking.Internal;

internal static class Dobby
{
    public enum Architecture
    {
        ARM,
        ARM64,
        X86,
        X64
    }

    public enum DobbyResult
    {
        Success = 0,
        Error = -1
    }

    public enum DobbyHookType
    {
        CodePatch,
        InlineHook
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DobbyRegisterContextARM
    {
        public uint dummy_0;
        public uint dummy_1;

        public uint dummy_2;
        public uint sp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public uint[] r;

        public uint lr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DobbyRegisterContextARM64
    {
        public ulong dmmpy_0;
        public ulong sp;

        public ulong dmmpy_1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 29)]
        public ulong[] x;

        public ulong fp;
        public ulong lr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public FPReg[] q;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DobbyRegisterContextX86
    {
        public uint dummy_0;
        public uint esp;

        public uint dummy_1;
        public uint flags;

        public X86Registers regs;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DobbyRegisterContextX64
    {
        public ulong dummy_0;
        public ulong rsp;

        public X64Registers regs;

        public ulong dummy_1;
        public ulong flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FPReg
    {
        public double d1;
        public double d2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct X86Registers
    {
        public uint eax, ebx, ecx, edx, ebp, esp, edi, esi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct X64Registers
    {
        public ulong rax, rbx, rcx, rdx, rbp, rsp, rdi, rsi, r8, r9, r10, r11, r12, r13, r14, r15;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DobbyInstrumentCallback(nint address, ref DobbyRegisterContextARM64 ctx);

    [DllImport("libDobby", CallingConvention = CallingConvention.Cdecl)]
    private static extern int DobbyCodePatch(nint address, byte[] buffer, uint bufferSize);

    [DllImport("libDobby", CallingConvention = CallingConvention.Cdecl)]
    private static extern int DobbyHook(nint address, nint replaceFunc, ref nint originFunc);

    [DllImport("libDobby", CallingConvention = CallingConvention.Cdecl)]
    private static extern int DobbyInstrument(nint address, DobbyInstrumentCallback preHandler);

    [DllImport("libDobby", CallingConvention = CallingConvention.Cdecl)]
    private static extern int DobbyDestroy(nint address);

    [DllImport("libDobby", CallingConvention = CallingConvention.Cdecl)]
    private static extern nint DobbyGetVersion();

    [DllImport("libDobby", CallingConvention = CallingConvention.Cdecl)]
    private static extern nint DobbySymbolResolver([MarshalAs(UnmanagedType.LPStr)] string imageName, [MarshalAs(UnmanagedType.LPStr)] string symbolName);

    [DllImport("libDobby", CallingConvention = CallingConvention.Cdecl)]
    private static extern int DobbyImportTableReplace([MarshalAs(UnmanagedType.LPStr)] string imageName, [MarshalAs(UnmanagedType.LPStr)] string symbolName,
                                                     nint fakeFunc, ref nint origFunc);

    public static DobbyResult CodePatch(nint address, byte[] buffer)
    {
        var result = DobbyCodePatch(address, buffer, (uint)buffer.Length);
        return result == 0 ? DobbyResult.Success : DobbyResult.Error;
    }

    public static DobbyResult Hook<T>(nint address, T replaceFunc, out T originFunc) where T : Delegate
    {
        var replaceFuncPtr = Marshal.GetFunctionPointerForDelegate(replaceFunc); 
        var originFuncPtr = nint.Zero;

        var result = DobbyHook(address, replaceFuncPtr, ref originFuncPtr);
        originFunc = Marshal.GetDelegateForFunctionPointer<T>(originFuncPtr);

        return result == 0 ? DobbyResult.Success : DobbyResult.Error;
    }

    public static DobbyResult Instrument(nint address, DobbyInstrumentCallback preHandler)
    {
        var result = DobbyInstrument(address, preHandler);
        return result == 0 ? DobbyResult.Success : DobbyResult.Error;
    }

    public static DobbyResult Destroy(nint address)
    {
        var result = DobbyDestroy(address);
        return result == 0 ? DobbyResult.Success : DobbyResult.Error;
    }

    public static string GetVersion()
    {
        var versionPtr = DobbyGetVersion();
        return Marshal.PtrToStringAnsi(versionPtr);
    }

    public static nint SymbolResolver(string imageName, string symbolName)
    {
        return DobbySymbolResolver(imageName, symbolName);
    }

    public static DobbyResult ImportTableReplace<T>(string imageName, string symbolName, T fakeFunc, out T origFunc) where T : Delegate
    {
        var fakeFuncPtr = Marshal.GetFunctionPointerForDelegate(fakeFunc);
        var origFuncPtr = nint.Zero;

        var result = DobbyImportTableReplace(imageName, symbolName, fakeFuncPtr, ref origFuncPtr);
        origFunc = Marshal.GetDelegateForFunctionPointer<T>(origFuncPtr);

        return result == 0 ? DobbyResult.Success : DobbyResult.Error;
    }
}

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Aetherium.Logging.Internal;

namespace Aetherium;
public static class SigScanner
{
    private static readonly ModuleLog Log = new("SigScanner");
    
    private const int BufferSize = 0x400000;

    private static class NativeMethods {
        public const int KERN_SUCCESS = 0;

        [DllImport("libSystem.dylib")]
        public static extern uint _dyld_image_count();

        [DllImport("libSystem.dylib")]
        public static extern nint _dyld_get_image_header(uint image_index);
    
        [DllImport("libSystem.dylib")]
        public static extern nint _dyld_get_image_name(uint image_index);

        [DllImport("libSystem.dylib")]
        public static extern int mach_task_self();

        [DllImport("libSystem.dylib")]
        public static extern int mach_vm_read_overwrite(int target_task, ulong address, ulong size, nint data, out ulong dataCnt);
    }

    public static nint FindPattern(string signature) =>
        FindPattern(ConvertHexStringToByteArray(signature));

    private static unsafe nint FindPattern(IReadOnlyList<byte?> signature) 
    {
        var num = NativeMethods._dyld_image_count();
        var self = NativeMethods.mach_task_self();
        var sigLen = signature.Count;
        var readMem = Marshal.AllocHGlobal(BufferSize + sigLen);

        for (uint i = 0; i < num; i++) 
        {
            var header = NativeMethods._dyld_get_image_header(i);
            var name = GetImageName(i) ?? "???";
            if (name.Contains("libAetherium.dylib")) continue;
            Log.Debug("Scanning image {image}", name);
            if (header == nint.Zero)
            {
                Log.Error("Image header {name} was NULL", name);
                continue;
            }

            var address = (ulong)header;

            while (NativeMethods.mach_vm_read_overwrite(self, address, BufferSize + (ulong)sigLen, readMem,
                       out var readMemCnt) == NativeMethods.KERN_SUCCESS) 
            {
                for (var j = 0; j < (int)readMemCnt - sigLen; j++)
                {
                    var candidate = (byte*)((long)readMem + j);
                    var match = true;
                    for (var k = 0; k < sigLen; k++)
                    {
                        if (signature[k] == null) continue;
                        if (candidate[k] == signature[k]) continue;
                        match = false;
                        break;
                    }
                    if (!match) continue;
                    Marshal.FreeHGlobal(readMem);
                    return new nint((long)address + j);
                }

                if (readMemCnt < BufferSize + (ulong)sigLen)
                    break;

                address += BufferSize;
            }
        }
        Log.Error("Could not find signature");
        Marshal.FreeHGlobal(readMem);
        return nint.Zero;
    }

    private static byte?[] ConvertHexStringToByteArray(string hexString)
    {
        var hexValuesSplit = hexString.Split(' ');
        var byteArray = new byte?[hexValuesSplit.Length];

        for (var i = 0; i < hexValuesSplit.Length; i++)
        {
            byteArray[i] = hexValuesSplit[i] == "??" ? null : byte.Parse(hexValuesSplit[i], NumberStyles.HexNumber);
        }

        return byteArray;
    }
    
    private static string? GetImageName(uint index)
    {
        var imageNamePtr = NativeMethods._dyld_get_image_name(index);
        return imageNamePtr == nint.Zero ? null : Marshal.PtrToStringAnsi(imageNamePtr);
    }
}

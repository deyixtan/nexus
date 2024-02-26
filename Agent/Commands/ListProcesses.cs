﻿using Agent.Models;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    public class ListProcesses : AgentCommand
    {
        public override string Name => "ps";

        public override string Execute(AgentTask task)
        {
            var results = new SharpSploitResultList<ListProcessesResult>();
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                var result = new ListProcessesResult
                {
                    ProcessName = process.ProcessName,
                    ProcessId = process.Id,
                    SessionId = process.SessionId
                };

                result.ProcessPath = GetProcessPath(process);
                result.Owner = GetProcessOwner(process);
                result.Arch = GetProcessArch(process);
                results.Add(result);
            }

            return results.ToString();
        }

        private string GetProcessPath(Process process)
        {
            try
            {
                return process.MainModule.FileName;
            }
            catch
            {
                return "-";
            }
        }

        private string GetProcessOwner(Process process)
        {
            IntPtr hToken = IntPtr.Zero;

            try
            {
                if (!Native.Advapi.OpenProcessToken(process.Handle, Native.Advapi.DesiredAccess.TOKEN_ALL_ACCESS, out hToken))
                {
                    return "-";
                }

                var identity = new WindowsIdentity(hToken);
                return identity.Name;
            }
            catch
            {
                return "-";
            }
            finally
            {
                Native.Kernel32.CloseHandle(hToken);
            }
        }

        private string GetProcessArch(Process process)
        {
            var is64BitOS = Environment.Is64BitOperatingSystem;
            if (!is64BitOS)
            {
                return "x86";
            }

            if (!Native.Kernel32.IsWow64Process(process.Handle, out var isWow64))
            {
                return "-";
            }

            return (is64BitOS && isWow64) ? "x64" : "x86";
        }
    }
    public sealed class ListProcessesResult : SharpSploitResult
    {
        public string ProcessName { get; set; }
        public string ProcessPath { get; set; }
        public string Owner {  get; set; }
        public int ProcessId { get; set; }
        public int SessionId { get; set; }
        public string Arch {  get; set; }

        protected internal override IList<SharpSploitResultProperty> ResultProperties => new List<SharpSploitResultProperty>
        {
            new SharpSploitResultProperty { Name = nameof(ProcessName), Value = ProcessName },
            new SharpSploitResultProperty { Name = nameof(ProcessPath), Value = ProcessPath },
            new SharpSploitResultProperty { Name = nameof(Owner), Value = Owner },
            new SharpSploitResultProperty { Name = "PID", Value = ProcessId },
            new SharpSploitResultProperty { Name = nameof(SessionId), Value = SessionId },
            new SharpSploitResultProperty { Name = nameof(Arch), Value = Arch }
        };
    }
}

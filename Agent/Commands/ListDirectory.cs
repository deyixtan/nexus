using Agent.Models;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    public class ListDirectory : AgentCommand
    {
        public override string Name => "ls";

        public override string Execute(AgentTask task)
        {
            var result = new SharpSploitResultList<ListDirectoryResult>();

            string path;

            if (task.Arguments is null || task.Arguments.Length == 0) 
            {
                path = Directory.GetCurrentDirectory();
            }
            else
            {
                path = task.Arguments[0];
            }

            // add files info
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                result.Add(new ListDirectoryResult
                {
                    Name = fileInfo.FullName,
                    Length = fileInfo.Length
                });
            }

            // add directories info
            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
            {
                var fileInfo = new FileInfo(directory);
                result.Add(new ListDirectoryResult
                {
                    Name = fileInfo.FullName,
                    Length = 0 // directory has no size
                });
            }

            return result.ToString();
        }
    }

    public sealed class ListDirectoryResult : SharpSploitResult
    {
        public string Name { get; set; }
        public long Length { get; set; }

        protected internal override IList<SharpSploitResultProperty> ResultProperties => new List<SharpSploitResultProperty>
        {
            new SharpSploitResultProperty { Name = nameof(Name), Value = Name },
            new SharpSploitResultProperty { Name = nameof(Length), Value = Length }
        };
    }
}

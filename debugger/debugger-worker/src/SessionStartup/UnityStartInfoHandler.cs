using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using JetBrains.Debugger.Worker;
using JetBrains.Debugger.Worker.Mono;
using JetBrains.Debugger.Worker.SessionStartup;
using JetBrains.Rider.Model.Unity.DebuggerWorker;
using Mono.Debugging.Autofac;
using Mono.Debugging.Client;
using Mono.Debugging.Client.DebuggerOptions;
using Mono.Debugging.Soft;

namespace JetBrains.ReSharper.Plugins.Unity.Rider.Debugger.SessionStartup
{
    [DebuggerGlobalComponent]
    public class UnityStartInfoHandler : ModelStartInfoHandlerBase<UnityStartInfo>
    {
        public UnityStartInfoHandler(UnityDebuggerWorkerHost host) : base(SoftDebuggerType.Instance)
        {
            // Host isn't used here but is required to force creation, registering the model
        }

        protected override IDebuggerSessionStarter GetSessionStarter(UnityStartInfo startInfo,
                                                                     IDebuggerSessionOptions debuggerSessionOptions)
        {
            var ipAddress = IPAddress.Loopback;
            var monoAddress = startInfo.MonoAddress;
            if (monoAddress != null)
            {
                try
                {
                    var hostAddress = Dns.GetHostAddresses(monoAddress);
                    ipAddress = hostAddress.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                }
                catch (Exception e)
                {
                    throw new DebuggerStartupException(e.Message, e);
                }

                if (ipAddress == null)
                    throw new DebuggerStartupException($"Host {monoAddress} cannot be resolved to any IP address");
            }

            var debuggerArgs = startInfo.ListenForConnections
                ? (SoftDebuggerRemoteArgs) new SoftDebuggerListenArgs(string.Empty, ipAddress, startInfo.MonoPort)
                : new SoftDebuggerConnectArgs(string.Empty, ipAddress, startInfo.MonoPort);
            return new RunSessionStarter(new SoftDebuggerStartInfo(debuggerArgs.SetConnectionProperties()),
                debuggerSessionOptions);
        }
    }
}
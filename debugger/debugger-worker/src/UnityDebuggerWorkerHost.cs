using JetBrains.Debugger.Worker;
using JetBrains.Lifetimes;
using JetBrains.Rider.Model.Unity.DebuggerWorker;
using Mono.Debugging.Autofac;

namespace JetBrains.ReSharper.Plugins.Unity.Rider.Debugger
{
    [DebuggerGlobalComponent]
    public class UnityDebuggerWorkerHost
    {
        public UnityDebuggerWorkerHost(Lifetime lifetime, IProtocols protocols)
        {
            Model = new UnityDebuggerWorkerModel(lifetime, protocols.Frontend);
        }

        public UnityDebuggerWorkerModel Model { get; }
    }
}
using System;
using Mono.Debugging.Autofac;

namespace JetBrains.ReSharper.Plugins.Unity.Rider.Debugger
{
    public interface IUnityOptions
    {
        bool ExtensionsEnabled { get; }
    }

    [DebuggerGlobalComponent]
    public class UnityOptions : IUnityOptions
    {
        private readonly UnityDebuggerWorkerHost myHost;

        public UnityOptions(UnityDebuggerWorkerHost host)
        {
            myHost = host;
            // ExtensionsEnabled = Environment.GetEnvironmentVariable("_RIDER_UNITY_ENABLE_DEBUGGER_EXTENSIONS") == "1";
        }

        public bool ExtensionsEnabled => myHost.Model.ShowCustomRenderers.Value;
    }
}
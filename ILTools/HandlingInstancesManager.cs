using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools
{
    public static class HandlingInstancesManager
    {
        private static readonly object sync = new object();
        private static readonly Dictionary<string, object> instaces = new Dictionary<string, object>();
        private static readonly Dictionary<string, List<InstanceRegisteredHandler>> instanceRegisteredHandlers = new Dictionary<string, List<InstanceRegisteredHandler>>();

        public static void RegisterInstance(string name, object instance)
        {
            lock (sync)
            {
                if (instaces.ContainsKey(name)) throw new InvalidOperationException("Instance with name '\{name}' was already registered.");
                instaces.Add(name, instance);
            }
        }

        private static void AddInstanceRegisteredEventHandler(string instanceName, InstanceRegisteredHandler instanceRegistered)
        {
            lock (sync)
            {
                List<InstanceRegisteredHandler> handlers;
                if (instanceRegisteredHandlers.ContainsKey(instanceName)) handlers = instanceRegisteredHandlers[instanceName];
                else handlers = new List<InstanceRegisteredHandler>();

                handlers.Add(instanceRegistered);
            }
        }

        public delegate void InstanceRegisteredHandler(object instance);
    }
}
